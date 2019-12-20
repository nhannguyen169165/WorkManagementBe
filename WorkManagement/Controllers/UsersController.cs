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
using Microsoft.AspNetCore.Authorization;

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
        [HttpGet,Authorize(Roles = "Admin")]
        [Route("GetAllUser/{AdminId}")]
        public async Task<IActionResult> GetAllUser([FromRoute] int AdminId,string type)
        {
            var user = _context.User;
            var userSort = _context.Authentication;
            string str = "";
            foreach(var item in user)
            {
                foreach(var x in userSort)
                {
                    if(x.Admin_id == AdminId)
                    {
                        if(x.User_id == item.Id)
                        {
                            var data = new { id = item.Id, email = item.Email, name = item.Fullname, password = item.Password, tagname = item.Tagname, role = item.Role, status = item.Status, TokenRegister = item.tokenRegister, TokenResetPassword = item.tokenResetPassword };
                            str += JsonConvert.SerializeObject(data) + ",";
                        }
                    }
                }
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
        [HttpGet,Authorize(Roles = "Admin")]
        [Route("GetUserDetail/{id}")]
        public async Task<IActionResult> GetUserDetail([FromRoute] int id)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.Id == id); 
            var data = new { id = user.Id, email = user.Email, name = user.Fullname, password = user.Password, tagname = user.Tagname, role = user.Role, TokenRegister = user.tokenRegister , status = user.Status};
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
        [HttpPut,Authorize(Roles = "Admin")]
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
                    thisUser.Role = item.Role;
                    thisUser.tokenRegister = item.tokenRegister;
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
        [Route("CreateUser"),Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] user user)
        {
            string str = "Tạo thành công";
            User u = new User();
            Authentication a = new Authentication();
            DateTime today = DateTime.Now;
            foreach (var item in user.userData)
            {
                u.Email = item.Email;
                u.Fullname = item.Fullname;
                u.Password = item.Password;
                u.Tagname = item.Tagname;
                u.Status = item.Status;
                u.Role = item.Role;
                u.tokenRegister = item.tokenRegister;
                u.tokenRegisterDate = item.tokenRegisterDate.ToLocalTime();
                u.tokenResetPasswordDate = today;
            }
            _context.User.Add(u);
            await _context.SaveChangesAsync();
            a.Admin_id = 1;
            a.User_id = u.Id;
            _context.Authentication.Add(a);
            await _context.SaveChangesAsync();
            var result = JsonConvert.SerializeObject(new { result = str });
           
            return Ok(result);
        }

        // DELETE: api/Users/5
        [HttpDelete,Authorize(Roles = "Admin")]
        [Route("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            var authentication = await _context.Authentication.FirstOrDefaultAsync(m => m.User_id == id);
            string str = "Xóa thành công";
            var result = JsonConvert.SerializeObject(new { result = str });
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                _context.User.Remove(user);
                _context.Authentication.Remove(authentication);
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
            var result = "invalid";
            if (user == null)
            {
               return Ok(JsonConvert.SerializeObject(new { Result = result }));
            }
            else
            {
                DateTime today = DateTime.Now;
                var createDate = user.tokenRegisterDate;
                var currentDate = Int32.Parse(today.Day + "" + today.Month + "" + today.Year);
                var currentTokenDate = Int32.Parse(createDate.Day + "" + createDate.Month + "" + createDate.Year);
                var currentHour = today.Hour;
                var currentMinute = today.Minute;
                var currentTokenHour = createDate.Hour;
                var currentTokenMinute = createDate.Minute;

                if (user.Status == "Active")
                {
                    result = "has-active";
                }
                else
                {
                    if (createDate.Hour > 12 && ((24 - createDate.Hour) + today.Hour) <= 12 && currentDate != currentTokenDate && (currentMinute < currentTokenMinute))
                    {
                        result = "valid";
                    }
                    if (currentDate == currentTokenDate && (currentHour - currentTokenHour) <= 12)
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

        [HttpGet]
        [Route("ValidateResetPassword/{token}")]
        public async Task<IActionResult> ValidateResetPassword([FromRoute] string token)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.tokenResetPassword == token);
            var result = "invalid";
            if (user == null)
            {
                return Ok(JsonConvert.SerializeObject(new { Result = result }));
            }
            else
            {
                DateTime today = DateTime.Now;
                var createDate = user.tokenResetPasswordDate;
                var currentDate = Int32.Parse(today.Day + "" + today.Month + "" + today.Year);
                var currentTokenDate = Int32.Parse(createDate.Day + "" + createDate.Month + "" + createDate.Year);
                var currentHour = today.Hour;
                var currentMinute = today.Minute;
                var currentTokenHour = createDate.Hour;
                var currentTokenMinute = createDate.Minute;
                if (user.statusResetPassword == "has-reset")
                {
                    result = "has-reset";
                }
                else
                {
                    if (createDate.Hour > 12 && ((24 - createDate.Hour) + today.Hour) <= 12 && currentDate != currentTokenDate && (currentMinute < currentTokenMinute))
                    {
                        result = "valid";
                    }
                    if (currentDate == currentTokenDate && (currentHour - currentTokenHour) <= 12)
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

        [HttpPost]
        [Route("CheckEmailExists")]
        public IActionResult checkEmailExists([FromBody] user user)
        {
            var u = _context.User;
            var email = "";
            var result = "";
            if (u == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var x in user.userData)
                {
                    email = x.Email;
                    foreach (var item in u)
                    {
                        if (item.Email == email)
                        {
                            result = JsonConvert.SerializeObject(new { Result = true, userId = item.Id });
                        }
                    }
                }
                return Ok(result);
            }
        }

        [HttpPut]
        [Route("EditUserNoAuth/{id}")]
        public async Task<IActionResult> EditUserNoAuth([FromRoute] int id, [FromBody] user user)
        {
            string str = "";
            var thisUser = await _context.User.SingleOrDefaultAsync(m => m.Id == id);
            foreach (var item in user.userData)
            {
                if (item.Password != null)
                {
                    thisUser.Password = item.Password;
                    thisUser.statusResetPassword = item.statusResetPassword;
                }
                else if (item.statusResetPassword != null)
                {
                    thisUser.statusResetPassword = item.statusResetPassword;
                    thisUser.tokenResetPassword = item.tokenResetPassword;
                    thisUser.tokenResetPasswordDate = item.tokenResetPasswordDate.ToLocalTime();
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

        // GET: api/Users
        [HttpGet]
        [Route("GetAllUserNoAuth")]
        public async Task<IActionResult> GetAllUserNoAuth(string type)
        {
            var user = _context.User;
            string str = "";
            foreach (var item in user)
            {
                var data = new { id = item.Id, TokenRegister = item.tokenRegister, TokenResetPassword = item.tokenResetPassword };
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
    }
}