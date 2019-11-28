using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkManagement.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using WorkManagement.Hubs;
using Microsoft.AspNetCore.Cors;

namespace WorkManagement.Controllers
{
    public class user
    {
        public User[] userData { get; set; }


    }
   
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
    
     
        private readonly WorkManagementContext _context;

        public UsersController(WorkManagementContext context)
        {
            _context = context;
        }

      
        // GET: api/Users
        [HttpGet]
        [Route("GetAllUser")]
        public async Task<IActionResult> GetAllUser(string type)
        {
            var user = _context.User;
            string str = "";
            foreach(var item in user)
            {
                var data = new { id = item.Id, email = item.Email, name = item.Fullname, password = item.Password, tagname = item.Tagname,status = item.Status, TokenRegister = item.tokenRegister , TokenResetPassword = item.tokenResetPassword};
                str += JsonConvert.SerializeObject(data) + ",";
            }
            str = str.Remove(str.Length - 1);
            str = "[" + str + "]";
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(str);

            }
        }

       // GET: api/Users/5
        [HttpGet]
        [Route("GetUserDetail/{id}")]
        public async Task<IActionResult> GetUserDetail([FromRoute] int id)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.Id == id); 
            var data = new { id = user.Id, email = user.Email, name = user.Fullname, password = user.Password, tagname = user.Tagname, TokenRegister = user.tokenRegister , status = user.Status};
            var str = JsonConvert.SerializeObject(data);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(str);

            }
        }

            // PUT: api/Users/5
        [HttpPut]
        [Route("EditUser/{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] user user)
        {
            string str = "";
            var thisUser = await _context.User.SingleOrDefaultAsync(m => m.Id == id);
            foreach (var item in user.userData)
            {
                if(item.Fullname != null  && item.Tagname != null)
                {
                    thisUser.Fullname = item.Fullname;
                    thisUser.Tagname = item.Tagname;
                    thisUser.Status = item.Status;
                }else if(item.Password != null)
                {
                    thisUser.Password = item.Password;
                    thisUser.statusResetPassword = item.statusResetPassword;
                }else if(item.statusResetPassword != null)
                {
                    thisUser.statusResetPassword = item.statusResetPassword;
                    thisUser.tokenResetPassword = item.tokenResetPassword;
                }
                else
                {
                    thisUser.Status = item.Status;
                } 
            }

            _context.User.Update(thisUser);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    str = "Người dùng không tồn tại !";
                }
                else
                {
                    str = "Cập nhật thành công";
                }
            }
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }

        // POST: api/Users
        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] user user)
        {
            string str = "Tạo thành công";
            User u = new User();
            DateTime today = DateTime.Now;
            foreach (var item in user.userData)
            {
                u.Email = item.Email;
                u.Fullname = item.Fullname;
                u.Password = item.Password;
                u.Tagname = item.Tagname;
                u.Status = item.Status;
                u.tokenRegister = item.tokenRegister;
                u.tokenRegisterDate = item.tokenRegisterDate.ToLocalTime();
                u.tokenResetPasswordDate = today;
            }
            _context.User.Add(u);
            var result = JsonConvert.SerializeObject(new { result = str });
            await _context.SaveChangesAsync();
            return Ok(result);
        }

        // DELETE: api/Users/5
        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            string str = "Xóa thành công";
            var result = JsonConvert.SerializeObject(new { result = str });
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                _context.User.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(result);
            }

           
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        [HttpGet]
        [Route("ValidateRegisterEmail/{token}")]
        public async Task<IActionResult> ValidateRegisterEmail([FromRoute] string token)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.tokenRegister == token);
            DateTime today = DateTime.Now;
            var createDate = user.tokenRegisterDate;
            var currentDate = Int32.Parse(today.Day + "" + today.Month + "" + today.Year);
            var currentTokenDate = Int32.Parse(createDate.Day + "" + createDate.Month + "" + createDate.Year);
            var currentHour = today.Hour;
            var currentMinute = today.Minute;
            var currentTokenHour = createDate.Hour;
            var currentTokenMinute = createDate.Minute;
            var result = "invalid";
            if (user.Status == "Active")
            {
                result = "has-active";
            }
            else
            {
                if (createDate.Hour == 12 && currentDate != currentTokenDate && currentMinute > 0)
                {
                    result = "valid";
                }
                if (currentDate == currentTokenDate && (currentHour - currentTokenHour) < 12)
                {
                    result = "valid";
                }
                if (currentDate == currentTokenDate && (currentHour - currentTokenHour) == 12 && (currentMinute < currentTokenMinute))
                {
                    result = "valid";
                }
                return Ok(JsonConvert.SerializeObject(new { Result = result }));
            }
            return Ok(JsonConvert.SerializeObject(new { Result = result }));
        }

        [HttpGet]
        [Route("ValidateResetPassword/{token}")]
        public async Task<IActionResult> ValidateResetPassword([FromRoute] string token)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.tokenResetPassword == token);
            DateTime today = DateTime.Now;
            var createDate = user.tokenResetPasswordDate;
            var currentDate = Int32.Parse(today.Day + "" + today.Month + "" + today.Year);
            var currentTokenDate = Int32.Parse(createDate.Day + "" + createDate.Month + "" + createDate.Year);
            var currentHour = today.Hour;
            var currentMinute = today.Minute;
            var currentTokenHour = createDate.Hour;
            var currentTokenMinute = createDate.Minute;
            var result = "invalid";
            if (user.statusResetPassword == "has-reset")
            {
                result = "has-reset";
            }
            else
            {
                if (createDate.Hour == 12 && currentDate != currentTokenDate && currentMinute > 0)
                {
                    result = "valid";
                }
                if (currentDate == currentTokenDate && (currentHour - currentTokenHour) < 12)
                {
                    result = "valid";
                }
                if (currentDate == currentTokenDate && (currentHour - currentTokenHour) == 12 && (currentMinute < currentTokenMinute))
                {
                    result = "valid";
                }
                return Ok(JsonConvert.SerializeObject(new { Result = result }));
            }
            return Ok(JsonConvert.SerializeObject(new { Result = result }));
        }
    }
}