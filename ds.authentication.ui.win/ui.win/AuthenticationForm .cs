//------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2020 Dassault Systèmes - CPE EMED
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
using ds.authentication.redirection;
using ds.enovia.model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ds.authentication.ui.win
{
    public partial class AuthenticationForm : Form
    {
        public AuthenticationForm(bool isCloud = false)
        {
            IsCloud = isCloud;

            InitializeComponent();

            InitUserSettings();

        }       

        private async void m_loginButton_Click(object sender, EventArgs e)
        {
            string enoviaURL   = EnoviaURL.Trim(' ');
            string passportURL = m_passportURLTextBox.Text.Trim(' ');
            string username    = m_usernameTextBox.Text.Trim(' ');
            string password    = m_passwordTextBox.Text;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                SaveUserSettings();

                DateTime startTime = DateTime.Now;

                // The first thing to do is to authenticate in the 3DEXPERIENCE Platform

                Passport = new UserPassport(passportURL);

                UserInfoRedirection userInfoRedirection = 
                        IsCloud ? new UserInfoRedirection(enoviaURL, m_tenantTextBox.Text) : new UserInfoRedirection(enoviaURL);

                userInfoRedirection.Current = true;
                userInfoRedirection.IncludeCollaborativeSpaces  = true;
                userInfoRedirection.IncludePreferredCredentials = true;

                UserInfo userInfo = await Passport.CASLoginWithRedirection<UserInfo>(username, password, false, userInfoRedirection);

                DateTime endTime = DateTime.Now;

                TimeSpan elapsedTime = endTime - startTime;

                this.Cursor = Cursors.Default;

                MessageBox.Show(string.Format("Successfully logged in {0} in {1:0.0} secs", userInfo.name, elapsedTime.TotalSeconds), "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                SecurityContextSelection securityContextSelection = new SecurityContextSelection();

               List<string> securityContextList =  GetSecurityContextList(userInfo.collabspaces);

               securityContextSelection.Initialize(securityContextList);

               if ((userInfo.preferredcredentials.collabspace == null) || ((userInfo.preferredcredentials.role == null)) || (userInfo.preferredcredentials.organization == null))
               {
                  securityContextSelection.SelectedContext = securityContextList[0];
               }
               else
               {
                  securityContextSelection.SelectedContext = userInfo.preferredcredentials.ToString();
               }

                if (DialogResult.OK != securityContextSelection.ShowDialog())
                {
                    throw new Exception("Login cancelled by the user");
                }

                this.SecurityContext = securityContextSelection.SelectedContext;

                UserInfo = userInfo;

                DialogResult = DialogResult.OK;

            }
            catch (Exception _ex)
            {
                this.Cursor = Cursors.Default;

                DialogResult = DialogResult.Abort;

                MessageBox.Show(_ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

                MessageBox.Show(_ex.StackTrace, "Exception Details", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
           
            this.Close();
        }

        public bool IsCloud { get; } = false;

        public string Tenant { get { return IsCloud ? m_tenantTextBox.Text : null; } }

        private void InitUserSettings()
        {
            if (IsCloud)
            {
                //m_premiseSecurityContextTextBox.Visible = false;
                //premiseLabel.Visible = false;

                m_passportURLTextBox.Text = Properties.Settings.Default.cua_passportURL;
                m_enoviaURLTextBox.Text = Properties.Settings.Default.cua_enoviaURL;
                m_usernameTextBox.Text = Properties.Settings.Default.cua_username;
                m_passwordTextBox.Text = Properties.Settings.Default.cua_password;
                m_tenantTextBox.Text = Properties.Settings.Default.cua_tenant;
                m_cloudSecurityContextTextBox.Text
                                          = Properties.Settings.Default.cua_securitycontext;
            }
            else
            {
                //m_cloudSecurityContextTextBox.Visible = false;
                cloudLabelTenant.Enabled = false;
                //cloudLabelSecContext.Visible = false;
                m_tenantTextBox.Enabled = false;

                m_passportURLTextBox.Text = Properties.Settings.Default.pua_passportURL;
                m_enoviaURLTextBox.Text = Properties.Settings.Default.pua_enoviaURL;
                m_usernameTextBox.Text = Properties.Settings.Default.pua_username;
                m_passwordTextBox.Text = Properties.Settings.Default.pua_password;
                m_tenantTextBox.Text = Properties.Settings.Default.cua_tenant;
                m_premiseSecurityContextTextBox.Text
                                          = Properties.Settings.Default.pua_securitycontext;

            }

        }

        private void SaveUserSettings()
        {
            if (IsCloud)
            {
                Properties.Settings.Default.cua_passportURL = m_passportURLTextBox.Text;
                Properties.Settings.Default.cua_enoviaURL = m_enoviaURLTextBox.Text;
                Properties.Settings.Default.cua_username = m_usernameTextBox.Text;
                Properties.Settings.Default.cua_securitycontext = m_cloudSecurityContextTextBox.Text;
                Properties.Settings.Default.cua_tenant = m_tenantTextBox.Text;
            }
            else
            {
                Properties.Settings.Default.pua_passportURL = m_passportURLTextBox.Text;
                Properties.Settings.Default.pua_enoviaURL = m_enoviaURLTextBox.Text;
                Properties.Settings.Default.pua_username = m_usernameTextBox.Text;
                Properties.Settings.Default.pua_securitycontext = m_premiseSecurityContextTextBox.Text;
            }

            Properties.Settings.Default.Save();
        }

        public UserInfo UserInfo { get; private set; } = null;

        public UserPassport Passport { get; private set; } = null;

        public string EnoviaURL { get { return m_enoviaURLTextBox.Text; } set { m_enoviaURLTextBox.Text = value; } }

        public string SecurityContext {

            get {   if (IsCloud)
                    {
                        return m_cloudSecurityContextTextBox.Text;
                    }
                    else
                    {
                        return m_premiseSecurityContextTextBox.Text;
                    }
            }

            set {
                    if (IsCloud)
                    {
                        m_cloudSecurityContextTextBox.Text = value;
                    }
                    else
                    {
                        m_premiseSecurityContextTextBox.Text = value;
                    }
            }
        }

        public bool IsValidAuthentication {
            get {
                return ((Passport != null) && (Passport.IsCookieAuthenticated));
        }}

        private bool IsSecurityContextValid(List<CollaborativeSpaceAccess> _collabspaces, string _securityContext)
        {
            //Check that Security Context is valid against the user access
            foreach (CollaborativeSpaceAccess collabSpaceAccess in _collabspaces)
            {
                string collabSpaceName = collabSpaceAccess.name;

                foreach (OrganizationRolePair orgRolePair in collabSpaceAccess.couples)
                {
                    Organization org = orgRolePair.organization;
                    Role role = orgRolePair.role;

                    string orgName = org.name;
                    string roleName = role.name;

                    //TODO: Could this be done with case insensitive?

                    string testSecurityContext = string.Format("{0}.{1}.{2}", roleName, org.name, collabSpaceName);

                    if (string.Equals(testSecurityContext, _securityContext))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private List<string> GetSecurityContextList(List<CollaborativeSpaceAccess> _collabspaces)
        {
            List<string> __securityContextList = new List<string>();

            //Check that Security Context is valid against the user access
            foreach (CollaborativeSpaceAccess collabSpaceAccess in _collabspaces)
            {
                string collabSpaceName = collabSpaceAccess.name;

                foreach (OrganizationRolePair orgRolePair in collabSpaceAccess.couples)
                {
                    Organization org = orgRolePair.organization;
                    Role role = orgRolePair.role;

                    string orgName = org.name;
                    string roleName = role.name;
                    
                    __securityContextList.Add(string.Format("{0}.{1}.{2}", roleName, org.name, collabSpaceName));
                }
            }

            return __securityContextList;
        }
    }
}
