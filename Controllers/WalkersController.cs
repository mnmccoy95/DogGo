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
    [Authorize]
    public class WalkersController : Controller
    {
        // GET: WalkersController
        public ActionResult Index()
        {
            return RedirectToAction("Details");
        }

        // GET: WalkersController/Details/5
        [Authorize]
        public ActionResult Details()
        {
            if (User.IsInRole("DogOwner"))
            {
                return NotFound();
            }
            else 
            { 
            int id = GetCurrentUserId();
            Walker walker = _walkerRepo.GetWalkerById(id);
            List<Walk> walks = _walkerRepo.GetWalksByWalkerId(id);
            List< Walk > walkSort = walks.OrderByDescending(walk => walk.owner.Name).ToList();
            WalkerProfileViewModel vm = new WalkerProfileViewModel()
            {
                Walker = walker,
                Walks = walkSort
            };

            return View(vm);
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

        public WalkersController(IOwnerRepository ownerRepo, IDogRepository dogRepo, IWalkerRepository walkerRepo, INeighborhoodRepository neighborRepo)
        {
            _ownerRepo = ownerRepo;
            _dogRepo = dogRepo;
            _walkerRepo = walkerRepo;
            _neighborhoodRepo = neighborRepo;
        }
    }
}
