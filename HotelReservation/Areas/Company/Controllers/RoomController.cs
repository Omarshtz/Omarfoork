﻿using AutoMapper;
using HotelReservation.Areas.Admin.Controllers;
using Infrastructures.Repository.IRepository;
using Infrastructures.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Utilities.Utility;

namespace HotelReservation.Areas.Company.Controllers
{
    [Area("Company")]
    [Authorize(Roles = SD.CompanyRole)]
    public class RoomsController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<RoomsController> logger;
        private readonly UserManager<IdentityUser> userManager;

        public RoomsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoomsController> logger, UserManager<IdentityUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
            this.userManager = userManager;
        }

        // GET: RoomsController
        public ActionResult Index(int id, string roomType = "All")
        {
            IEnumerable<Room> rooms;

            if (id != 0)
            {
                Response.Cookies.Append("HotelIdCookie", id.ToString());
            }
            else
            {
                id = int.Parse(Request.Cookies["HotelIdCookie"]);
            }

            rooms = unitOfWork.RoomRepository.Get(
                where: e => e.HotelId == id && (roomType == "All" || e.RoomType.Type.ToString() == roomType),
                include: [e => e.Hotel, w => w.RoomType]
            );

            ViewBag.HotelId = id;
            ViewBag.RoomType = roomType;
            return View(rooms);
        }


        // GET: RoomsController/Details/5
        public ActionResult Details(int id)
        {
            var room = unitOfWork.RoomRepository.GetOne(
                [e => e.Hotel, w => w.RoomType],
                where: r => r.Id == id
            );

            if (room == null)
            {
                return RedirectToAction("NotFound", "Home", new { area = "Customer" });
            }

            return View(room);
        }
        // GET: RoomsController/Create
        public ActionResult Create(int hotelId)
        {

            ViewBag.HotelId = hotelId;
            ViewBag.Type = unitOfWork.RoomTypeRepository.Get(where: t => t.HotelId == hotelId);
            return View();
        }

        // POST: RoomController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Room room, int count)
        {
            if (ModelState.IsValid)
            {
                var rooms = new List<Room>();
                for (int i = 0; i < count; i++)
                {
                    var newRoom = mapper.Map<Room>(room);
                    rooms.Add(newRoom);
                }
                Log(nameof(Create), nameof(room) + " " + $"count: {count}");
                unitOfWork.RoomRepository.AddRange(rooms);
                unitOfWork.Complete();
                TempData["success"] = $"Successfully created {count} room(s).";

                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }
        // POST: RoomsController/Book/5
        public ActionResult Book(int id)
        {
            var room = unitOfWork.RoomRepository.GetOne(where: r => r.Id == id);
            if (room == null)
           {
                return RedirectToAction("NotFound", "Home", new { area = "Customer" });
           }

           room.IsAvailable = !room.IsAvailable;
            Log(nameof(Book), nameof(room) + " " + $"Book: {room.IsAvailable}");
            unitOfWork.RoomRepository.Update(room);
           unitOfWork.Complete();
            TempData["success"] = room.IsAvailable ? "Room is now available." : "Room is now unavailable.";


            return RedirectToAction(nameof(Index), new { id = room.HotelId });
        }

        public ActionResult Delete(int id)
        {
            var room = unitOfWork.RoomRepository.GetOne(
               [e => e.Hotel, w => w.RoomType],
                where: a => a.Id == id
            );

            if (room == null)
            {
                return RedirectToAction("NotFound", "Home", new { area = "Customer" });
            }

            return View(room);
        }

        // POST: RoomsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Room room)
        {
            if (room == null)
            {
                return RedirectToAction("NotFound", "Home", new { area = "Customer" });
            }
            Log(nameof(Delete), nameof(room) + " " + $"Id: {room.Id}");
            unitOfWork.RoomRepository.Delete(room);
            unitOfWork.Complete();
            TempData["success"] = "Room deleted successfully.";

            return RedirectToAction(nameof(Index), new { id = room.HotelId });
        }
        public async void Log(string action, string entity)
        {
            var user = await userManager.GetUserAsync(User);
            LoggerHelper.LogAdminAction(logger, user.Id, user.Email, action, entity);
        }
    }
}
