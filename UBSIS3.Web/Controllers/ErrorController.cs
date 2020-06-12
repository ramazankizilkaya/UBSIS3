using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using UBSIS3.Web.Data.Interfaces;

namespace UBSIS3.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IStringLocalizer<ErrorController> _localizer;
        private readonly IErrorLogRepository _errorRepo;

        public ErrorController(IStringLocalizer<ErrorController> localizer, IErrorLogRepository errorRepo)
        {
            _localizer = localizer;
            _errorRepo = errorRepo;
        }


        [Route("/Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            ViewData["Title"] = _localizer["Error"];

            ViewBag.Home = _localizer["Home"];
            ViewBag.Error= _localizer["Error"];
            ViewBag.Title= _localizer["Error"];
            ViewBag.Description= _localizer["Error occured. The page you were looking for, couldn't be found. We will handle it. Thanks for your patience..."];

            ViewBag.Oops= _localizer["Oops"];
            //rgdrg
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionDetails!=null)
            {
                _errorRepo.CreateEntity(new Models.ErrorLog
                {
                    ErrorLocation = exceptionDetails.Path,
                    ErrorDetail = exceptionDetails.Error.Message ?? "",
                    StatusCode = statusCode
                });
            }
            else
            {
                _errorRepo.CreateEntity(new Models.ErrorLog
                {
                    StatusCode = statusCode
                });
            }
            
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = _localizer["The page you were looking for couldn't be found. We will handle it. Thanks for your patience..."];
                    ViewBag.ErrorCode = statusCode;
                    break;
                default:
                    ViewBag.ErrorMessage = _localizer["Something went wrong. We will handle it. Thanks for your patience..."];
                    ViewBag.ErrorCode = statusCode;
                    break;
            }
            return View("NotFound");
        }

        [Route("/Exception")]
        [AllowAnonymous]
        public IActionResult Exception()
        {
            ViewData["Title"] = _localizer["Exception"];

            ViewBag.Home = _localizer["Home"];
            ViewBag.Exception = _localizer["Exception"];
            ViewBag.Title = _localizer["Exception"];
            ViewBag.Description = _localizer["Exception occured. The page you were looking for, couldn't be found. We will handle it. Thanks for your patience..."];
            ViewBag.Oops = _localizer["Oops"];

            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionDetails != null)
            {
                _errorRepo.CreateEntity(new Models.ErrorLog
                {
                    ErrorLocation = exceptionDetails.Path,
                    ErrorDetail = exceptionDetails.Error.Message ?? "",
                    StatusCode = 0
                });
            }
            
            return View("Error");
        }
    }
}