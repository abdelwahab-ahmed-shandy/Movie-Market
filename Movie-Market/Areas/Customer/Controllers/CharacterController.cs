﻿using BLL.Services.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CharacterController : BaseController
    {
        private readonly ICustomerCharacterService _characterService;

        public CharacterController(ICustomerCharacterService characterService)
        {
            _characterService = characterService;
        }

        public async Task<IActionResult> Index()
        {
            var characters = await _characterService.GetAllCharactersAsync();
            return View(characters);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var character = await _characterService.GetCharacterDetailsAsync(id);
            if (character == null)
            {
                return NotFound();
            }
            return View(character);
        }
    }
}
