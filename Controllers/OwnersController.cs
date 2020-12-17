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
    public class OwnersController : Controller
    {
        // GET: OwnerController
        [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("Details");
        }

        // GET: OwnerController/Details/5
        [Authorize]
        public ActionResult Details()
        {
            if (User.IsInRole("DogWalker"))
            {
                return NotFound();
            }
            else
            {
                int id = GetCurrentUserId();
                Owner owner = _ownerRepo.GetOwnerById(id);
                List<Dog> dogs = _dogRepo.GetDogsByOwnerId(owner.Id);
                List<Walker> walkers = _walkerRepo.GetWalkersInNeighborhood(owner.NeighborhoodId);

                ProfileViewModel vm = new ProfileViewModel()
                {
                    Owner = owner,
                    Dogs = dogs,
                    Walkers = walkers
                };

                return View(vm);
            }
        }

        // GET: OwnerController/Create
        public ActionResult Create()
        {
            List<Neighborhood> neighborhoods = _neighborhoodRepo.GetAll();

            OwnerFormViewModel vm = new OwnerFormViewModel()
            {
                Owner = new Owner(),
                Neighborhoods = neighborhoods
            };

            return View(vm);
        }

        // POST: OwnerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OwnerFormViewModel ownerFormModel)
        {
            try
            {
                _ownerRepo.AddOwner(ownerFormModel.Owner);

                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, ownerFormModel.Owner.Id.ToString()),
                    new Claim(ClaimTypes.Email, ownerFormModel.Owner.Email),
                    new Claim(ClaimTypes.Role, "DogOwner"),
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction(nameof(Details), new { id = ownerFormModel.Owner.Id });
            }
            catch
            {
                ownerFormModel.Neighborhoods = _neighborhoodRepo.GetAll();

                return View(ownerFormModel);
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

        public OwnersController(IOwnerRepository ownerRepo, IDogRepository dogRepo, IWalkerRepository walkerRepo, INeighborhoodRepository neighborRepo)
        {
            _ownerRepo = ownerRepo;
            _dogRepo = dogRepo;
            _walkerRepo = walkerRepo;
            _neighborhoodRepo = neighborRepo;
        }
    }
}
