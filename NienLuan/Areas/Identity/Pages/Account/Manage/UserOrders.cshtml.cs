using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPYte.Data;
using SPYte.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace SPYte.Areas.Identity.Pages.Account.Manage
{
    public class UserOrdersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserOrdersModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public string Username { get; set; }
        public string UserId { get; set; }

        public Pager pager { get; set; }

        public List<UserOrder> Orders { get; set; }


        [TempData]
        public string StatusMessage { get; set; }


        private async Task LoadAsync(ApplicationUser user, int pg)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);

            Username = userName;
            UserId = userId;

            var shshopdbContext = await _context.UserOrders.Where(m => m.UserId == UserId).ToListAsync();
            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = shshopdbContext.Count();

            pager = new Pager(recsCount, pg, pageSize);

            ViewData["Pager"] = pager;

            int recSkip = (pg - 1) * pageSize;

            var orders = shshopdbContext.Skip(recSkip).Take(pager.PageSize);


            Orders = shshopdbContext;
        }

        public async Task<IActionResult> OnGet(int pg = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user, pg);


            return Page();

        }
    }
}
