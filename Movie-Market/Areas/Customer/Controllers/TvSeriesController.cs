﻿using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class TvSeriesController : BaseController
    {
        private readonly ITvSeriesService _tvSeriesService;

        public TvSeriesController(ITvSeriesService tvSeriesService, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _tvSeriesService = tvSeriesService;
        }


        public async Task<IActionResult> Index()
        {
            var tvSeries = await _tvSeriesService.GetAllTvSeriesAsync();
            return View(tvSeries);
        }


        public async Task<IActionResult> Details(Guid id)
        {
            var tvSeriesDetails = await _tvSeriesService.GetTvSeriesWithDetailsAsync(id);

            if (tvSeriesDetails == null)
            {
                return NotFound();
            }

            return View(tvSeriesDetails);
        }


    }
}
