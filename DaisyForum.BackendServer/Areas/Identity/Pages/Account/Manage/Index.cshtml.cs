// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DaisyForum.BackendServer.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IFileValidator _fileValidator;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<IndexModel> logger,
            IWebHostEnvironment env,
            IFileValidator fileValidator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _env = env;
            _fileValidator = fileValidator;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Display(Name = "Tài khoản")]
        public string Username { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string Avatar { get; set; }

        public string UploadAvatarErrorMessage { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public IFormFile AvatarFile { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Required]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Ngày sinh")]
            [Required]
            [DataType(DataType.Date)]
            public DateTime Dob { get; set; }

            [Display(Name = "Họ")]
            [Required]
            public string FirstName { get; set; }

            [Display(Name = "Tên")]
            [Required]
            public string LastName { get; set; }

            [Display(Name = "Giới thiệu")]
            public string Description { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            Avatar = user.Avatar;
            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
                   .Replace(" ", "")
                   .Replace("-", ""),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Dob = user.Dob,
                Description = user.Description,

            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải được người dùng: '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải được người dùng: '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.FirstName != user.FirstName)
            {
                user.FirstName = Input.FirstName;
                await _userManager.UpdateAsync(user);
            }

            if (Input.LastName != user.LastName)
            {
                user.LastName = Input.LastName;
                await _userManager.UpdateAsync(user);
            }

            if (Input.Dob != user.Dob)
            {
                user.Dob = Input.Dob;
                await _userManager.UpdateAsync(user);
            }

            if (Input.Description != user.Description)
            {
                user.Description = Input.Description;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Thông tin của bạn đã được cập nhật";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAvatarAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            if (!_fileValidator.IsValid(AvatarFile))
            {
                UploadAvatarErrorMessage = "Invalid file.";
                await LoadAsync(user);
                return Page();
            }

            // Upload new picture
            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(AvatarFile.FileName);
            var uploaded = await UploadAvatarAsync(AvatarFile, fileName);
            if (uploaded)
            {
                // Delete the previous one
                if (!string.IsNullOrEmpty(user.Avatar))
                    DeleteAvatar(user.Avatar);

                // Update database
                user.Avatar = fileName;
                await _userManager.UpdateAsync(user);
            }

            StatusMessage = "Thông tin của bạn đã được cập nhật";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAvatarAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var deleted = DeleteAvatar(user.Avatar);
            if (deleted)
            {
                user.Avatar = null;
                await _userManager.UpdateAsync(user);
            }

            StatusMessage = "Thông tin của bạn đã được cập nhật";
            return RedirectToPage();
        }

        private async Task<bool> UploadAvatarAsync(IFormFile file, string fileName)
        {
            try
            {
                var folderPath = Path.Combine(_env.WebRootPath, "avatars");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, Path.GetFileName(fileName));
                using var fs = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fs);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa ảnh đại diện: {file.FileName}.", ex.Message);
            }

            return false;
        }

        private bool DeleteAvatar(string fileName)
        {
            try
            {
                var folderPath = Path.Combine(_env.WebRootPath, "avatars");
                var filePath = Path.Combine(folderPath, Path.GetFileName(fileName));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa ảnh đại diện: {fileName}.", ex.Message);
            }

            return false;
        }
    }
}
