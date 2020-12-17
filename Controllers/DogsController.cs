using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogGo.Repositories;
using DogGo.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace DogGo.Controllers
{
    [Authorize]
    public class DogsController : Controller
    {
        // GET: DogsController/Details/5
        public ActionResult Details(int id)
        {
            if (User.IsInRole("DogWalker"))
            {
                return NotFound();
            }
            else
            { 
                Dog dog = _dogRepo.GetDogById(id);
                if(dog.OwnerId == GetCurrentUserId())
                {
                    return View(dog);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        // GET: DogsController/Create
        public ActionResult Create()
        {
            if (User.IsInRole("DogWalker"))
            {
                return NotFound();
            }
            else
            { 
                return View();
            }
        }

        // POST: DogsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Dog dog)
        {
            if (User.IsInRole("DogWalker"))
            {
                return NotFound();
            }
            else
            { 
                try
                {
                    // update the dogs OwnerId to the current user's Id
                    dog.OwnerId = GetCurrentUserId();

                    _dogRepo.AddDog(dog);

                    return RedirectToAction("Index", "Owners");
                }
                catch (Exception ex)
                {
                    return View(dog);
                }
            }
        }

        // GET: DogsController/Edit/5
        public ActionResult Edit(int id)
        {
            if (User.IsInRole("DogWalker"))
            {
                return NotFound();
            }
            else
            { 
                Dog dog = _dogRepo.GetDogById(id);
                if(dog.OwnerId == GetCurrentUserId())
                {
                    return View(dog);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        // POST: DogsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Dog dog)
        {
            if(User.IsInRole("DogWalker"))
            {
                return NotFound();
            }
            else
            { 
                if (dog.OwnerId == GetCurrentUserId())
                { 
                    try
                    {
                        dog.OwnerId = GetCurrentUserId();
                        _dogRepo.UpdateDog(dog);

                        return RedirectToAction("Index", "Owners");
                    }
                    catch (Exception ex)
                    {
                        return View(dog);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
        }

        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }

        private readonly IDogRepository _dogRepo;

        // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
        public DogsController(IDogRepository dogRepository)
        {
            _dogRepo = dogRepository;
        }
    }
}
