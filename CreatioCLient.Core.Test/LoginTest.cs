using CreatioClient.Core.Models.Domain;
using CreatioClient.Core.Models.Dto;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ICredentials = CreatioClient.Core.Models.Domain.ICredentials;

namespace CreatioClient.Core.Test
{
    public class LoginTests
    {
        private HttpClient _client;
        ICredentials _credentials = new Credentials("", "");

        [Test]
        public async Task Login_Should_SucceedOnGoodLogin()
        {
            #region Arrange
            _client = MockHttpClient.Create(DesiredResponses.LoginSucceeded);
            
            Services.Login login = new(_client, _credentials);
            #endregion

            #region Act
            var actual = await login.Execute();
            #endregion

            #region Assert
            Assert.AreEqual(0, actual.Code);
            Assert.AreEqual("", actual.Message);
            Assert.AreEqual(null, actual.PasswordChangeUrl);
            Assert.AreEqual(null, actual.RedirectUrl);
            #endregion
        }

        [Test]
        public async Task Login_Should_Succed_OnBadModel_OrWrongCredentials()
        {
            #region Arrange
            _client = MockHttpClient.Create(DesiredResponses.LoginFailedWithWrongRequest);
            Services.Login login = new(_client, _credentials);

            Exception expectedException = new Exception(null, null, "Security error.", null, "System.Security.SecurityException");

            #endregion

            #region Act
            var actual = await login.Execute();
            #endregion

            #region Assert
            Assert.AreEqual(1, actual.Code);
            Assert.AreEqual("Error Message", actual.Message);
            Assert.AreEqual(expectedException.Type, actual.Exception.Type);
            Assert.AreEqual(expectedException.Message, actual.Exception.Message);

            Assert.AreEqual(null, actual.PasswordChangeUrl);
            Assert.AreEqual(null, actual.RedirectUrl);
            #endregion
        }

    }
}