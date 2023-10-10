using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Security.Claims;
using webapi.LicenseModels;
using webapi.Services;
using System.ComponentModel;

namespace webapi.Controllers
{
    [Consumes("application/xml")]
    [ApiController]
    [Route("AuthController")]
    public class AuthController : ControllerBase
    {
        
        private readonly IUserLicenseService _userLicenseService;

        public AuthController(IUserLicenseService userLicenseService)
        {
            _userLicenseService = userLicenseService;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            // Extract user credentials from the request
            string username = loginRequest.Username;
            string password = loginRequest.Password;
            string licenseXml = loginRequest.LicenseXml;
            string requiredRole = loginRequest.Role; // Extract required role from the request

            // Deserialize the XML string representing the license into a License object
            LicenseModels.License license;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(LicenseModels.License));
                using (TextReader reader = new StringReader(licenseXml))
                {
                    license = (LicenseModels.License)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                // Handle deserialization error
                return BadRequest(new { message = "Invalid license format" });
            }

            // Call IsValidLicense with all required parameters
            if (_userLicenseService.IsValidLicense(license, username, password, requiredRole, null))
            {
                // License is valid, perform login actions here

                // Store user information in the session
                HttpContext.Session.SetString("Username", username);
                // You can store additional information as needed

                return Ok(new { message = "Login successful" });
            }

            // License is not valid, return unauthorized status
            return Unauthorized(new { message = "Invalid license or insufficient privileges" });
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            // Clear user information from the session
            HttpContext.Session.Clear();
            return Ok(new { message = "Logout successful" });
        }
    }
}
