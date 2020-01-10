﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WorkManagement.Models;

namespace WorkManagement.Controllers
{
    public class project
    {
        public Project[] projectData;
        public StatusProject[] statusData;
    }
    public class listUser
    {
        public ListUserInProject[] listUserData;
    }
    public class statusProject
    {
        public StatusProject[] statusData;
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly WorkManagementContext _context;

        public ProjectsController(WorkManagementContext context)
        {
            _context = context;
        }

        // GET: api/Projects
        [HttpGet,Authorize(Roles = "Project Manager")]
        [Route("GetProject/{UserId}")]
        public async Task<IActionResult> GetProjects([FromRoute] int UserId)
        {
            var project = _context.Project;
            string str = "";
            if (project == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var item in project)
                {
                    if (item.User_id == UserId)
                    {
                        var data = new { id = item.Id, name = item.Name, description = item.Description, status = item.Status, progress = 100 };
                        str += JsonConvert.SerializeObject(data) + ",";
                    }
                }
                if (str == "")
                {
                    return Ok("[]");
                }
                str = str.Remove(str.Length - 1);
                str = "[" + str + "]";
                return Ok(str);
            }
        }

        // GET: api/Projects/5
        [HttpGet("{id}"),Authorize(Roles = "Project Manager")]
        [Route("GetProjectDetail/{id}")]
        public async Task<IActionResult> GetProjectDetail([FromRoute] int id)
        {
            var project = await _context.Project.SingleOrDefaultAsync(m => m.Id == id);
            var data = new { id = project.Id,name = project.Name,description = project.Description};
            var str = JsonConvert.SerializeObject(data);
            if (project == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(str);

            }
        }

        // PUT: api/Projects/5
        [HttpPut]
        [Route("EditProject/{id}")]
        public async Task<IActionResult> UpdateProject([FromRoute] int id, [FromBody] project project)
        {
            string str = "";
            var thisProject = await _context.Project.SingleOrDefaultAsync(m => m.Id == id);
            foreach (var item in project.projectData)
            {
                if(thisProject.Name != null && thisProject.Description != null)
                {
                    thisProject.Name = item.Name;
                    thisProject.Description = item.Description;
                }
            }

            _context.Project.Update(thisProject);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    str = "Project no exists !";
                }
                else
                {
                    str = "Updated project successfully";
                }
            }
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }

        // POST: api/Projects
        [HttpPost]
        [Route("AddProject")]
        public async Task<IActionResult> AddProject([FromBody] project project)
        {
            string str = "Create project successfully";
            Project p = new Project();
            foreach(var item in project.projectData)
            {
                p.Name = item.Name;
                p.Description = item.Description;
                p.Status = "null";
                p.User_id = item.User_id;
            }
            _context.Project.Add(p);
            await _context.SaveChangesAsync();
            foreach (var item in project.statusData)
            {
                StatusProject sp = new StatusProject();
                sp.StatusName = item.StatusName;
                sp.Serial = item.Serial;
                sp.Relation = item.Relation;
                sp.ProjectId = p.Id;
                _context.StatusProject.Add(sp);
            }
            await _context.SaveChangesAsync();
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }

       
        // GET: api/Users
        [HttpGet, Authorize(Roles = "Project Manager")]
        [Route("GetAllMember/{AdminId},{UserId}")]
        public async Task<IActionResult> GetAllMember([FromRoute] int AdminId, int UserId)
        {
          
            var userData = (from auth in _context.Authentication
                        join user in _context.User
                        on auth.User_id equals user.Id
                        join admin in _context.Admin
                        on auth.Admin_id equals admin.Id
                        select new
                        {
                            Id = auth.User_id,
                            FullName = user.Fullname,
                            Email = user.Email,
                            TagName = user.Tagname,
                            Status = user.Status,
                            AdminId = auth.Admin_id
                       
                        });
            string str = "";
            foreach (var item in userData)
            {
                if(item.Id != UserId)
                {
                    if (item.AdminId == AdminId)
                    {
     
                        if (item.Status == "Active")
                        {
                            var data = new { id = item.Id, email = item.Email, name = item.FullName, tagname = item.TagName };
                            str += JsonConvert.SerializeObject(data) + ",";
                        }
                    }
                        
                }
            }
            if (str == "")
            {
                return Ok("[]");
            }
            str = str.Remove(str.Length - 1);
            str = "[" + str + "]";
            return Ok(str);
        }

        // GET: api/Users
        [HttpGet, Authorize(Roles = "Project Manager")]
        [Route("GetProjectMember/{ProjectId}")]
        public async Task<IActionResult> GetProjectMember([FromRoute] int ProjectId, string type)
        {
           
            
            var listMember = (from listMem in _context.ListUserInProject
                              join user in _context.User
                              on listMem.User_id equals user.Id
                              select new
                              {
                                Id = listMem.Id,
                                ProjectId = listMem.Project_Id,
                                Uid = user.Id,
                                FullName = user.Fullname,
                                Email = user.Email,
                                TagName = user.Tagname

                             });
            if(listMember == null)
            {
                return NotFound();
            }else
            {
                string str = "";
                foreach (var item in listMember)
                {
                    if (item.ProjectId == ProjectId)
                    {
                        var data = new { id = item.Id, uid = item.Uid, email = item.Email, name = item.FullName, tagname = item.TagName };
                        str += JsonConvert.SerializeObject(data) + ",";   
                    }
                }
                if(str == "")
                {
                    return Ok("[]");
                }
                str = str.Remove(str.Length - 1);
                str = "[" + str + "]";
                return Ok(str);
            }
         
        }

        // POST: api/Projects
        [HttpPost]
        [Route("AddMember"), Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> AddMember([FromBody] listUser listUser)
        {
            string str = "Add member successfully";
            foreach (var item in listUser.listUserData)
            {
                ListUserInProject listUserProject = new ListUserInProject();
                listUserProject.Project_Id = item.Project_Id;
                listUserProject.User_id = item.User_id;
                _context.ListUserInProject.Add(listUserProject);
            }
            await _context.SaveChangesAsync();
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }

        // DELETE: api/Projects/5
        [HttpDelete]
        [Route("DeleteMember/{id}")]
        public async Task<IActionResult> DeleteMember([FromRoute] int id) { 
            string str = "Delete member successfully";
            var result = JsonConvert.SerializeObject(new { result = str });
            var listUserProject = await _context.ListUserInProject.FirstOrDefaultAsync(m => m.Id == id); 
            if (listUserProject == null)
                {
                return NotFound();
            }
            _context.ListUserInProject.Remove(listUserProject);
            await _context.SaveChangesAsync();
            return Ok(result);
        }
            

      

        // DELETE: api/Projects/5
        [HttpDelete, Authorize(Roles = "Project Manager")]
        [Route("DeleteProject/{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] int id)
        {
            var project = await _context.Project.FirstOrDefaultAsync(m => m.Id == id);
            var listMember = _context.ListUserInProject;
            string str = "Xóa thành công";
            var result = JsonConvert.SerializeObject(new { result = str });
            if (project == null)
            {
                return NotFound();
            }
            else
            {
                _context.Project.Remove(project);
                foreach (var lm in listMember)
                {
                    if (lm.Project_Id == id)
                    {
                        _context.ListUserInProject.Remove(lm);
                    }
                }
                await _context.SaveChangesAsync();
                return Ok(result);
            }
     
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }

    }
}