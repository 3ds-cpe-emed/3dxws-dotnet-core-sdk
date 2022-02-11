//------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2021 Dassault Systèmes - CPE EMED
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished
// to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//------------------------------------------------------------------------------------------------------------------------------------


using ds.authentication;
using ds.enovia.common.search;
using ds.enovia.document.exception;
using ds.enovia.document.model;
using ds.enovia.service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ds.enovia.document.service
{
    public class DocumentService : EnoviaBaseService
    {
        private const string BASE_RESOURCE = "/resources/v1/modeler/documents";
        private const string FILES = "/files";
        private const string FILES_CHECKIN = FILES + "/CheckinTicket";

        private const string PARENT = "/parentId";
        private const string DOWNLOAD_TICKET = "/DownloadTicket";

        private const string SEARCH = "/search";

        private const string ATTACHMENT           = "Reference Document";
        private const string SPECIFICATION_LEGACY = "PLMDocConnection";
        private const string SPECIFICATION        = "SpecificationDocument";

        private const string SOURCE = "from";

        private static ConcurrentBag<string> m_activeFileUploads = new ConcurrentBag<string>();

        public string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public DocumentService(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {

        }

        #region Public Interface

        public async Task<DocumentResponse<Document>> Search(SearchQuery _searchString)
        {
            return await _Search(_searchString.GetSearchString());
        }

        public async Task<DocumentResponse<DocumentCreated>> CreateDocument(string _title, string _description, string _fileLocalPath, string _collabSpace = null, string _documentType = null)
        {
            //TODO: Check that the _fileLocalPath exists and the process has read permissions

            string filename = System.IO.Path.GetFileName(_fileLocalPath);

            string uploadFileReceipt = await UploadFile(_fileLocalPath);

            DocumentDataCreate docData = InitializeDocument(_title, _description, _collabSpace, _documentType);

            docData = AddFileToDocument(docData, filename, uploadFileReceipt);

            return await _CreateDocument(docData);
        }

        public async Task<DocumentResponse<DocumentCreated>> CreateDocumentAsSpecification(string _title, string _description, string _parentId, string _fileLocalPath, string _collabSpace = null)
        {
            //TODO: Check that the _fileLocalPath exists and the process has read permissions

            string filename = System.IO.Path.GetFileName(_fileLocalPath);

            string uploadFileReceipt = await UploadFile(_fileLocalPath);

            DocumentDataCreate docData = InitializeDocument(_title, _description, _collabSpace);

            docData = AddFileToDocument(docData, filename, uploadFileReceipt);

            return await CreateDocumentAsSpecification(docData, _parentId);
        }

        public async Task<DocumentResponse<DocumentCreated>> CreateDocumentAsAttachment(string _title, string _description, string _parentId, string _fileLocalPath, string _collabSpace = null)
        {
            //TODO: Check that the _fileLocalPath exists and the process has read permissions

            string filename = System.IO.Path.GetFileName(_fileLocalPath);

            string uploadFileReceipt = await UploadFile(_fileLocalPath);

            DocumentDataCreate docData = InitializeDocument(_title, _description, _collabSpace);

            docData = AddFileToDocument(docData, filename, uploadFileReceipt);

            return await CreateDocumentAsAttachment(docData, _parentId);
        }


        public async Task<DocumentResponse<Document>> GetDocument(string _docId)
        {
            string getDocument = $"{GetBaseResource()}/{_docId}";

            HttpResponseMessage requestResponse = await GetAsync(getDocument);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new GetDocumentsFromParentException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<DocumentResponse<Document>>();
        }

        public async Task<DocumentResponse<Document>> GetAttachedDocuments(string _parentId)
        {
            return await GetConnectedDocuments(_parentId, SOURCE, ATTACHMENT);
        }

        public async Task<DocumentResponse<Document>> GetSpecificationDocuments(string _parentId)
        {
            return await GetConnectedDocuments(_parentId, SOURCE, SPECIFICATION);
        }

        public async Task<bool> DeleteDocument(string _physicalId)
        {
            string deleteResource = string.Format("{0}/{1}", GetBaseResource(), _physicalId);

            HttpResponseMessage deleteDocumentResponse = await DeleteAsync(deleteResource);

            if (deleteDocumentResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new DeleteDocumentException(deleteDocumentResponse));
            }
            return true;
        }

        //DownloadFile
        public async Task<bool> GetFileFromDocument(string _documentPId, string _fileId, string _downloadLocation)
        {
            //Get Download Ticket
            string endpoint = string.Format("{0}/{1}{2}/{3}{4}", GetBaseResource(), _documentPId, FILES, _fileId, DOWNLOAD_TICKET);

            HttpResponseMessage response = await PutAsync(endpoint);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new DownloadTicketException(response));
            }

            TicketResponse<FileDownloadTicketData> ticketResponse =
                await response.Content.ReadFromJsonAsync<TicketResponse<FileDownloadTicketData>>();

            string filename = ticketResponse.data[0].dataelements.fileName;

            if (!_downloadLocation.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                _downloadLocation += System.IO.Path.DirectorySeparatorChar.ToString();
            }

            string downloadFile = string.Format("{0}{1}", _downloadLocation, filename);

            using (var writer = File.OpenWrite(downloadFile))
            {
                //var request = new RestRequest(ticketResponse.data[0].dataelements.ticketURL);
                //request.ResponseWriter = responseStream =>
                //{
                //    using (responseStream)
                //    {
                //        responseStream.CopyTo(writer);
                //    }
                //};

                //var downloadDataReturn = m_client.DownloadData(request);

                //THIS NEEDS TO BE CHANGED - check using a static httpclient or IHttpClientFactory (include timeout)
                using (HttpClient downloadClient = new HttpClient(new HttpClientHandler()))
                {
                    using (HttpResponseMessage res = await downloadClient.GetAsync(ticketResponse.data[0].dataelements.ticketURL))
                    using (Stream streamToReadFrom = await res.Content.ReadAsStreamAsync())
                    {
                        streamToReadFrom.CopyTo(writer);
                    }
                }

            }

            return true;
        }
        public async Task<FileInfo> DownloadFileFromDocument(HttpClient _downloadHttpClient, string _documentPId, string _fileId, string _downloadLocation)
        {
            //Get Download Ticket
            string endpoint = string.Format("{0}/{1}{2}/{3}{4}", GetBaseResource(), _documentPId, FILES, _fileId, DOWNLOAD_TICKET);

            HttpResponseMessage response = await PutAsync(endpoint);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new DownloadTicketException(response));
            }

            TicketResponse<FileDownloadTicketData> ticketResponse =
                await response.Content.ReadFromJsonAsync<TicketResponse<FileDownloadTicketData>>();

            string filename = ticketResponse.data[0].dataelements.fileName;

            if (!_downloadLocation.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                _downloadLocation += System.IO.Path.DirectorySeparatorChar.ToString();
            }

            string downloadFile = string.Format("{0}{1}", _downloadLocation, filename);

            string downloadUrl = ticketResponse.data[0].dataelements.ticketURL;

            HttpResponseMessage res = await _downloadHttpClient.GetAsync(downloadUrl);

            using (var writer = File.OpenWrite(downloadFile))
            {
                using (Stream streamToReadFrom = await res.Content.ReadAsStreamAsync())
                {
                    streamToReadFrom.CopyTo(writer);
                }
                //}
            }

            return new FileInfo(downloadFile);

        }
        #endregion

        //Important: Queries must not exceed 4096 characters.
        private async Task<DocumentResponse<Document>> _Search(string _searchString)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            //queryParams.Add("$include", _include.GetString());
            //queryParams.Add("$fields", _fields.ToString());
            queryParams.Add("searchStr", _searchString);

            string searchResource = string.Format("{0}{1}", GetBaseResource(), SEARCH);

            HttpResponseMessage requestResponse = await GetAsync(searchResource, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                // When results are empty this seems to be sending this error. Need to open an SR.
                // After a while it starts working again so I suspect this has to do with deleting documents 
                // until the indexing had not been redone
                // I will comment out the workaround nonetheless

                //JObject errParser = JObject.Parse(requestResponse.Content);

                //if (errParser.ContainsKey("error"))
                //{
                //    string errorValue = errParser["error"].Value<string>();

                //    if (errorValue.Equals("Error: Invalid Business Object Specified.", StringComparison.InvariantCultureIgnoreCase))
                //    {
                //        return new DocumentResponse<Document>();
                //    }
                //};

                throw (new DocumentSearchException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<DocumentResponse<Document>>();
        }

        private async Task<DocumentResponse<Document>> GetConnectedDocuments(string _parentId, string _parentDirection, string _parentRelName)
        {
            string getDocumentsFromParent = string.Format("{0}{1}/{2}", GetBaseResource(), PARENT, _parentId);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("parentRelName", _parentRelName);
            queryParams.Add("parentDirection", _parentDirection);

            HttpResponseMessage requestResponse = await GetAsync(getDocumentsFromParent, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new GetDocumentsFromParentException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<DocumentResponse<Document>>();
        }

        private async Task<DocumentResponse<DocumentCreated>> CreateDocumentAsAttachment(DocumentDataCreate _docData, string _parentId)
        {
            return await CreateConnectedDocument(_docData, _parentId, SOURCE, ATTACHMENT);
        }
        private async Task<DocumentResponse<DocumentCreated>> CreateDocumentAsSpecification(DocumentDataCreate _docData, string _parentId)
        {
            return await CreateConnectedDocument(_docData, _parentId, SOURCE, SPECIFICATION);
        }

        private DocumentDataCreate InitializeDocument(string _title, string _description, string _collabSpace = null, string _type = null)
        {
            DocumentDataCreate _docData = new DocumentDataCreate();
            _docData.dataelements.title = _title;
            _docData.dataelements.description = _description;

            if (_type != null)
                _docData.type = _type;

            if (_collabSpace != null)
                _docData.dataelements.collabspace = _collabSpace;

            return _docData;
        }

        private DocumentDataCreate ConnectDocument(DocumentDataCreate _docData, string _parentId, string _parentDirection, string _parentConnectionName)
        {
            //Add parent 
            _docData.dataelements.parentId = _parentId;
            _docData.dataelements.parentDirection = _parentDirection;
            _docData.dataelements.parentRelName = _parentConnectionName;

            return _docData;
        }

        private DocumentDataCreate AddFileToDocument(DocumentDataCreate _docData, string _fileTitle, string _fileUploadReceipt)
        {
            if (_docData.relateddata == null)
                _docData.relateddata = new DocumentRelatedDataCreate();

            _docData.relateddata.files.Add(new DocumentRelatedDataFilesCreate());
            _docData.relateddata.files[0].dataelements.title = _fileTitle;
            _docData.relateddata.files[0].dataelements.receipt = _fileUploadReceipt;

            return _docData;
        }

        private async Task<DocumentResponse<DocumentCreated>> CreateConnectedDocument(DocumentDataCreate _docData, string _parentId, string _parentDirection, string _parentConnectionName)
        {
            _docData = ConnectDocument(_docData, _parentId, _parentDirection, _parentConnectionName);

            return await _CreateDocument(_docData);
        }

        private async Task<DocumentResponse<DocumentCreated>> _CreateDocument(DocumentDataCreate _docData)
        {
            DocumentCreate doc = new DocumentCreate();

            doc.data.Add(_docData);

            string bodyRequest = JsonSerializer.Serialize(doc);

            HttpResponseMessage createDocumentResponse = await PostAsync(GetBaseResource(), null, null, bodyRequest);

            if (createDocumentResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new CreateDocumentException(createDocumentResponse));
            }

            return await createDocumentResponse.Content.ReadFromJsonAsync<DocumentResponse<DocumentCreated>>();
        }

        private async Task<string> UploadFile(string _fileLocalPath)
        {
            //Get the FCS Checkin Ticket
            string filesCheckInResource = string.Format("{0}{1}", GetBaseResource(), FILES_CHECKIN);

            HttpResponseMessage requestResponse = await PutAsync(filesCheckInResource);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new CheckInException(requestResponse));
            }

            FileCheckInTicket fileCheckInTicket =
                await requestResponse.Content.ReadFromJsonAsync<FileCheckInTicket>();

            //Upload file
            FileCheckInTicketData data = fileCheckInTicket.data[0];

            string ticketParamName = data.dataelements.ticketparamname;
            string ticket = data.dataelements.ticket;
            string ticketUrl = data.dataelements.ticketURL;

            #region Prepare Message Content
            
            //----
            // Building the request using Multipart Form Data
            // Create the file content part by copying the file contents
            var fileContent = new StreamContent(File.OpenRead(_fileLocalPath));
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"file_0\"",
                FileName = "\"" + Path.GetFileName(_fileLocalPath) + "\""
            };
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            // Create the ticket content
            StringContent ticketContent = new StringContent(ticket);
            ticketContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = $"\"{ticketParamName}\""
            };

            // -----
            var requestContent = new MultipartFormDataContent();
            requestContent.Add(ticketContent);
            requestContent.Add(fileContent);

            //Note: This was VERY hard to find however the 3DEXPERIENCE does not accept double quotes in the boundary
            //definition... Need to remove them.
            NameValueHeaderValue boundary = requestContent.Headers.ContentType.Parameters.First(o => o.Name == "boundary");
            boundary.Value = boundary.Value.Replace("\"", String.Empty);

            #endregion

            #region Finalize and deliver message

            // Prepare request resource path / address --------
            Uri ticketUrlUri = new Uri(ticketUrl);
            
            string baseAddress = $"{ticketUrlUri.Scheme}://{ticketUrlUri.Host}/";
            
            if (!ticketUrlUri.IsDefaultPort)
            {
                baseAddress += $":{ticketUrlUri.Port}";
            }

            HttpClient client = new HttpClient(new HttpClientHandler()) { BaseAddress = new Uri(baseAddress) };
       
            HttpResponseMessage result = await client.PostAsync(ticketUrlUri.AbsolutePath, requestContent);
            
            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw new UploadFileException(result);
            }
            
            #endregion

            string __receipt = await result.Content.ReadAsStringAsync();

            return __receipt.Trim('\n'); ;
        }
    }
}