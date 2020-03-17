﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using POSBlazor.Data.Services.Common;
using VinaC2C.Data.Context;
using VinaC2C.Data.Models;
using VinaC2C.Data.Services.Feature;
using VinaC2C.Data.Services.Mail;
using VinaC2C.Data.Services.User;
using VinaC2C.MVC.Models;
using VinaC2C.MVC.ServerHub;
using VinaC2C.Ultilities.AppInfor;
using VinaC2C.Ultilities.Enums;
using VinaC2C.Ultilities.Helpers;

namespace VinaC2C.MVC.Controllers
{
    public class FeatureController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        private readonly VinaC2CContext _context;

        public FeatureService featureService;


        private IHubContext<SignalServer> _hubContext;

        public FeatureController(VinaC2CContext context, IWebHostEnvironment environment, IHubContext<SignalServer> hubContext)
        {
            this._context = context;
            this._environment = environment;
            this._hubContext = hubContext;
            featureService = new FeatureService(context);
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<JsonResult> GetAllAsyncToList()
        {
            return Json(await featureService.GetAllToListAsync());
        }

        public JsonResult Create(Feature feature)
        {
            feature.Initialization(ObjectInitType.Insert, "");
            int result = featureService.Create(feature);
            _hubContext.Clients.All.SendAsync("dataChangeNotification", null);
            if (result != 0)
                return Json(new { messageType = "success", note = AppGlobal.InsertSuccessMessage });
            else
                return Json(new { messageType = "error", note = AppGlobal.InsertFailMessage });
        }

        public JsonResult Update(Feature feature)
        {
            feature.Initialization(ObjectInitType.Update, "");
            int result = featureService.Update(feature.Id, feature);
            _hubContext.Clients.All.SendAsync("dataChangeNotification", null);
            if (result != 0)
                return Json(new { messageType = "success", note = AppGlobal.UpdateSuccessMessage });
            else
                return Json(new { messageType = "error", note = AppGlobal.UpdateFailMessage });
        }

        public JsonResult Delete(Feature feature)
        {
            int result = featureService.Delete(feature.Id);
            _hubContext.Clients.All.SendAsync("dataChangeNotification", null);
            if (result != 0)
                return Json(new { messageType = "success", note = AppGlobal.DeleteSuccessMessage });
            else
                return Json(new { messageType = "error", note = AppGlobal.DeleteFailMessage });
        }
    }
}