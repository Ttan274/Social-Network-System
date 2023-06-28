﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TESNS.Models.Authentication;
using TESNS.Models;
using TESNS.ViewModels;
using TESNS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TESNS.Controllers
{
    //[Authorize]
    public class PostController: Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IPhotoService _photoService;

        public PostController(ApplicationDbContext context, UserManager<AppUser> userManager, IPhotoService photoService,SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _photoService = photoService;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult CreatePost()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostViewModel postVM)
        {
            var result = await _photoService.AddPhotoAsync(postVM.ImagePath);
            Post newPost = new Post()
            {
                Header = postVM.Header,
                Text = postVM.Text,
                //ImagePath = result.Url.ToString(),
                Video = postVM.Video,
                PublishDate = DateTime.Now.ToUniversalTime(),
                EditedDate = DateTime.Now.ToUniversalTime(),
                CommentCount = 0,
                LikeCount = 0
            };
            if(postVM.ImagePath != null)
            {
                newPost.ImagePath = result.Url.ToString();
            }
            
            var currentUser = await _userManager.GetUserAsync(User);
            newPost.UserId = currentUser.Id;
            
            //newPost.CommunityId = _context.Communities.FirstOrDefault().Id;

            _context.Posts.Add(newPost);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ListPosts()
        {
            var user = await _userManager.GetUserAsync(User);
            var posts = await _context.Posts.Where(x => x.UserId == user.Id).ToListAsync();
            return View(posts);
        }

        /*[HttpGet]
        public IActionResult AddCommunity()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCommunity(Community com)
        {
            Community comi = new Community()
            {
                Logo = com.Logo,
                Description = com.Description,
                InviteLink = com.InviteLink
            };

            _context.Communities.Add(comi);
            _context.SaveChanges();


            return RedirectToAction("CreatePost", "Post");
        }*/
    }
}
