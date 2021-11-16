using ds.authentication;
using ds.enovia.common.collection;
using ds.enovia.common.constants;
using ds.enovia.dsdo.mask;
using ds.enovia.dsdo.model;
using ds.enovia.service;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace ds.enovia.dsdo
{
    public class DerivedOutputService : EnoviaBaseService
    {
        private const string LOCATE = "/resources/v1/modeler/dsdo/dsdo:DerivedOutputs/Locate";
        private const string CREATE = "/resources/v1/modeler/dsdo/dsdo:DerivedOutputs";
        private const string GET = "/resources/v1/modeler/dsdo/dsdo:DerivedOutputs";
        private const string DOWNLOAD_TICKET = @"/resources/v1/modeler/dsdo/dsdo:DerivedOutputs/{0}/dsdo:DerivedOutputFiles/{1}/DownloadTicket";
        private const string UPLOAD_TICKET = @"/resources/v1/modeler/dsdo/CheckinTicket";

        public DerivedOutputService(string enoviaService, IPassportAuthentication passport) : base(enoviaService, passport)
        {
        }

        //public async Task<DerivedOutputsCreated> Create(string _ownerType, string _ownerId, string _filename, string _format)
        //{
        //    //Upload file first (includes getting the upload ticket)
        //    string uploadReceipt = await this.Upload(SecurityContext, _ownerId, _filename);

        //    //TODO: Query Params
        //    //_mask.GetString();

        //    ////Prepare the body of the payload with the reference to the object that owns the Derived Outputs
        //    //DerivedOutputsReferences derivedOutputsRefs = new DerivedOutputsReferences();
        //    //derivedOutputsRefs.referencedObject.Add(new DerivedOutputRefObject(_type, _id));
        //    DerivedOutputFileCreate fileCreate = new DerivedOutputFileCreate();
        //    fileCreate.receipt = uploadReceipt;
        //    fileCreate.format = _format; // hardcoded for now
        //    fileCreate.filename = Path.GetFileName(_filename);
        //    fileCreate.checksum = "f517695d7da2bca30e889c5a37933a79"; // GetMD5Checksum(_filename);

        //    DerivedOutputRefObject doRefObj = new DerivedOutputRefObject(_ownerType, _ownerId);

        //    DerivedOutputsCreate doCreate = new DerivedOutputsCreate();
        //    doCreate.referencedObject = doRefObj;
        //    doCreate.derivedoutputfiles = new List<DerivedOutputFileCreate>();
        //    doCreate.derivedoutputfiles.Add(fileCreate);

        //    string body = doCreate.ToJson();

        //    IRestResponse requestResponse = await PostAsync(CREATE, null, null, body);

        //    if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
        //    {
        //        //handle according to established exception policy
        //    //    throw (new DerivedOutputException(requestResponse));
        //    }

        //    return SimpleJson.DeserializeObject<DerivedOutputsCreated>(requestResponse.Content);
        //}


        //Get information of all derived outputs.
        //public async Task<DerivedOutputsLocateResponse> Locate(string _id, string _type = null, DerivedOutputsMask _mask = DerivedOutputsMask.Details)
        public async Task<ItemSet<DerivedOutputsLocate>> LocateAsync(DerivedOutputsReferences _derivedOutputsRefs, 
            DerivedOutputsMask _mask = DerivedOutputsMask.Details)        
        {
            //TODO: Query Params
            //Dictionary<string, string> queryParams = new Dictionary<string, string>();
            //queryParams.Add("$mask", _mask.GetString());

            string body = JsonSerializer.Serialize(_derivedOutputsRefs);

            //string body = _derivedOutputsRefs.toJson();

            HttpResponseMessage requestResponse = await PostAsync(LOCATE, null, null, body, true, false);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new DerivedOutputException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<ItemSet<DerivedOutputsLocate>>(); 
        }

        //public async Task<DerivedOutputFileUploadTicketResponse> GetUploadTicketAsync(string _securityContext, string _ownerId)
        //{
        //    //TODO: Validate input

        //    //Setup the HTTP headers, notably for this call the Content-Type which is required (not documented at the time of this writing r2020xFD05)
        //    //even if the body payload is empty.
        //    Dictionary<string, string> headers = new Dictionary<string, string>();

        //    headers.Add(HttpRequestHeaders.SECURITY_CONTEXT, _securityContext);
        //    headers.Add(HttpRequestHeaders.ACCEPT, MimeTypes.APPLICATION_JSON);
        //    headers.Add(HttpRequestHeaders.CONTENT_TYPE, MimeTypes.APPLICATION_JSON); //!!!! This is mandatory even if the POST is empty R2020xFD05 !!!

        //    DerivedOutputsReferenceFileCheckinRequest fileCheckinRequest = new DerivedOutputsReferenceFileCheckinRequest(1, _ownerId);

        //    string body = SimpleJson.SerializeObject(fileCheckinRequest);

        //    //Send http post request
        //    IRestResponse requestResponse = await PostAsync(UPLOAD_TICKET, IncludeTenant, null, headers, body, true, true);

        //    if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
        //    {
        //        //handle according to established exception policy
        //        throw (new DerivedOutputException(requestResponse));
        //    }

        //    return SimpleJson.DeserializeObject<DerivedOutputFileUploadTicketResponse>(requestResponse.Content);
        //}

        public async Task<DerivedOutputFileDownloadTicketResponse> GetDownloadTicketAsync(string _doid, string _dofid)
        {
            string doFIdEncodedUTF8 = HttpUtility.UrlEncode(_dofid);

            //TODO: Validate input
            string downloadTicketEndpoint = string.Format(DOWNLOAD_TICKET, _doid, doFIdEncodedUTF8);

            //Send http post request
            //This is required to set the Content-Type to application/json even if there is no body being sent
            HttpResponseMessage requestResponse = await PostAsync(downloadTicketEndpoint, null, null, "");

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new DerivedOutputException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<DerivedOutputFileDownloadTicketResponse>();
        }

        //private void Download(FileDownloadTicket _downloadTicket, string _localFolder)
        //{
        //    string downloadEscapedTicket = Uri.EscapeDataString(_downloadTicket.ticket);

        //    string downloadUrl = string.Format("{0}?__fcs__jobTicket={1}", _downloadTicket.ticketURL, downloadEscapedTicket);

        //    if (!_localFolder.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
        //    {
        //        _localFolder += System.IO.Path.DirectorySeparatorChar.ToString();
        //    }

        //    string filename = string.Format("{0}{1}", _localFolder, _downloadTicket.filename);

        //    // REST SHARP Sample - Working with files https://restsharp.dev/usage/files.html
        //    // Here's an example that will use a Stream to avoid memory buffering of request content. 
        //    // Useful when retrieving large amounts of data that you will be immediately writing to disk

        //    using (var writer = File.OpenWrite(filename))
        //    {
        //        var request = new RestRequest(downloadUrl);

        //        request.ResponseWriter = responseStream =>
        //        {
        //            using (responseStream)
        //            {
        //                responseStream.CopyTo(writer);
        //            }
        //        };

        //        var response = m_client.DownloadData(request);
        //    }

        //}

        //public async Task<bool> Download(string _securityContext, string _doId, string _dofId, string _localFolder)
        //{
        //    DerivedOutputFileDownloadTicketResponse doFileDownloadTicketResponse = await GetDownloadTicketAsync(_securityContext, _doId, _dofId);

        //    FileDownloadTicket fileDownloadTicket = doFileDownloadTicketResponse.data.dataelements;

        //    Download(fileDownloadTicket, _localFolder);

        //    return true;
        //}

        //private async Task<string> Upload(string _securityContext, FileUploadTicket _uploadTicket, string _filename)
        //{
        //    //TODO: Validate params (e.g. filename exists?)
        //    //filename needs to start with the format e.g. PDF_....

        //    //Setup the HTTP headers, notably for this call the Content-Type which is required (not documented at the time of this writing r2020xFD05)
        //    //even if the body payload is empty.
        //    Dictionary<string, string> headers = new Dictionary<string, string>();
        //    headers.Add(HttpRequestHeaders.SECURITY_CONTEXT, _securityContext);

        //    Dictionary<string, string> formParamsDictionary = new Dictionary<string, string>();
        //    formParamsDictionary.Add("__fcs__jobTicket", _uploadTicket.ticket);

        //    Dictionary<string, string> fileParamsDictionary = new Dictionary<string, string>();
        //    fileParamsDictionary.Add("file", _filename);

        //    IRestResponse uploadFileRequest =
        //        await PostMultipartAsync(_uploadTicket.ticketURL, IncludeTenant, null, headers, formParamsDictionary, fileParamsDictionary, false);

        //    if (!uploadFileRequest.IsSuccessful)
        //    {
        //        // TODO: Throw
        //    }

        //    return uploadFileRequest.Content;

        //}
        //public async Task<string> Upload(string _securityContext, string _doId, string _filename)
        //{
        //    //1st - get a checkin receipt
        //    DerivedOutputFileUploadTicketResponse derivedOutputsResponse = await GetUploadTicketAsync(_securityContext, _doId);

        //    if ((derivedOutputsResponse.success != "true") || (derivedOutputsResponse.statusCode != "200"))
        //    {
        //        //TODO: Throw
        //    }

        //    FileUploadTicket fileUploadTicket = derivedOutputsResponse.data.dataelements;

        //    //2nd - upload the file and get the receipt

        //    return await Upload(_securityContext, fileUploadTicket, _filename);

        //}
    }
}
