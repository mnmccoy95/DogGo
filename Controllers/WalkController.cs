using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogGo.Repositories;
using DogGo.Models;
using DogGo.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace DogGo.Controllers
{
    public class WalkController : Controller
    {
        // GET: WalkController
        public ActionResult Index()
        {
            return View();
        }

        // GET: WalkController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: WalkController/Create
        public ActionResult Create(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);
            if (User.IsInRole("DogWalker"))
            {
                return NotFound();
            }
            else
            {
                int userId = GetCurrentUserId();
                List<Dog> dogs = _dogRepo.GetDogsByOwnerId(userId);

                WalkFormViewModel vm = new WalkFormViewModel()
                {
                    walk = new Walk(),
                    dogs = dogs
                };
                vm.walk.walker = walker;

                return View(vm);
            }
        }

        // POST: WalkController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WalkFormViewModel walkForm)
        {
           
                _walkRepository.AddWalk(walkForm.walk);
                return RedirectToAction("Index", "Owners");
            
            
        }

        // GET: WalkController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: WalkController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
        private IOwnerRepository _ownerRepo;
        private IDogRepository _dogRepo;
        private IWalkerRepository _walkerRepo;
        private INeighborhoodRepository _neighborhoodRepo;
        private IWalkRepository _walkRepository;

        public WalkController(IOwnerRepository ownerRepo, IDogRepository dogRepo, IWalkerRepository walkerRepo, INeighborhoodRepository neighborRepo, IWalkRepository walkRepository)
        {
            _ownerRepo = ownerRepo;
            _dogRepo = dogRepo;
            _walkerRepo = walkerRepo;
            _neighborhoodRepo = neighborRepo;
            _walkRepository = walkRepository;
        }
    }
}
