using System;
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
    public class ProjectModel
    {
        public Project[] projectData;
        public StatusProject[] statusData;
    }
    public class ListUserModel
    {
        public ListUserInProject[] listUserData;
    }
    public class StatusProjectModel
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
        public async Task<IActionResult> GetProject([FromRoute] int UserId)
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
                        var data = new { id = item.Id, name = item.Name, description = item.Description, color = item.Color, status = item.Status, progress = 100 };
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
            var data = new { id = project.Id,name = project.Name,description = project.Description,startDate = project.StartDate,finishDate = project.FinishDate,workingTimePerDay = project.WorkingTimePerDay,workingDayPerWeek = project.WorkingDayPerWeek,color = project.Color};
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
        public async Task<IActionResult> UpdateProject([FromRoute] int id, [FromBody] ProjectModel project)
        {
            string str = "";
            var thisProject = await _context.Project.SingleOrDefaultAsync(m => m.Id == id);
            foreach (var item in project.projectData)
            {
                if(thisProject.Name != null && thisProject.Description != null)
                {
                    thisProject.Name = item.Name;
                    thisProject.Description = item.Description;
                    thisProject.StartDate = item.StartDate;
                    thisProject.FinishDate = item.FinishDate;
                    thisProject.WorkingTimePerDay = item.WorkingTimePerDay;
                    thisProject.WorkingDayPerWeek = item.WorkingDayPerWeek;
                    thisProject.Color = item.Color;
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
        public async Task<IActionResult> AddProject([FromBody] ProjectModel project)
        {
            string str = "Create successfully";
            Project p = new Project();
            foreach(var item in project.projectData)
            {
                p.Name = item.Name;
                p.Description = item.Description;
                p.StartDate = item.StartDate;
                p.FinishDate = item.FinishDate;
                p.WorkingTimePerDay = item.WorkingTimePerDay;
                p.WorkingDayPerWeek = item.WorkingDayPerWeek;
                p.Color = item.Color;
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
        [HttpGet]
        [Route("GetAllMember/{AdminId},{UserId}")]
        public async Task<IActionResult> GetAllMember([FromRoute] int AdminId, int UserId)
        {
            var listMemberData = (from auth in _context.Authentication
                            join listMem in _context.ListUserInProject
                            on auth.User_id equals listMem.User_id
                            join admin in _context.Admin.Where(a => a.Id == AdminId)
                            on auth.Admin_id equals admin.Id
                                  select new
                            {
                                Id = auth.User_id
                            });
            var userData = (from auth in _context.Authentication.Where(a => a.User_id != UserId)
                            join user in _context.User
                        on auth.User_id equals user.Id
                        join admin in _context.Admin.Where(a => a.Id == AdminId)
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
                if (listMemberData.Count() == 0)
                {
                    if (item.Status == "Active")
                    {
                        var data = new { id = item.Id, email = item.Email, name = item.FullName, tagname = item.TagName, adminId = item.AdminId };
                        str += JsonConvert.SerializeObject(data) + ",";
                    }
                
                }
                else
                {
                    foreach (var listMem in listMemberData)
                    {

                        if (item.Status == "Active" && item.Id != listMem.Id)
                        {
                            var data = new { id = item.Id, email = item.Email, name = item.FullName, tagname = item.TagName, adminId = item.AdminId };
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
        public async Task<IActionResult> GetProjectMember([FromRoute] int ProjectId)
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
                                TagName = user.Tagname,
                                Color = user.Color

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
                        var data = new { id = item.Id, uid = item.Uid, email = item.Email, name = item.FullName, tagname = item.TagName,color = item.Color };
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
        public async Task<IActionResult> AddMember([FromBody] ListUserModel listUser)
        {
            string str = "Add successfully";
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
            string str = "Delete successfully";
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
            string str = "Delete successfully";
            var result = JsonConvert.SerializeObject(new { result = str });
            if (project == null)
            {
                return NotFound();
            }
            else
            {
                _context.Project.Remove(project);
                await _context.SaveChangesAsync();
                return Ok(result);
            }
     
        }

        [HttpGet]
        [Route("GetListStatus/{ProjectId}")]
        public async Task<IActionResult> GetListStatus([FromRoute] int ProjectId)
        {
            var listStatus = _context.StatusProject.OrderBy((m) => m.Serial).Where((s) => s.ProjectId == ProjectId);
            var strStatus = "";
            foreach (var status in listStatus)
            {
                var statusProjectData = new { id = status.Id, statusName = status.StatusName};
                strStatus += JsonConvert.SerializeObject(statusProjectData) + ",";

            }
            if (strStatus == "")
            {
                return Ok("[]");
            }
            else
            {
                strStatus = strStatus.Remove(strStatus.Length - 1);
                strStatus = "[" + strStatus + "]";
            }
            return Ok(strStatus);

        }

        [HttpPost, Authorize(Roles = "Project Manager")]
        [Route("AddNewStatus")]
        public async Task<IActionResult> AddNewStatus([FromBody] StatusProjectModel status)
        {

            string str = "Create Successfully";
            StatusProject statusProject = new StatusProject();
            foreach (var item in status.statusData)
            {

                statusProject.StatusName = item.StatusName;
                statusProject.Serial = item.Serial;
                statusProject.ProjectId = item.ProjectId;
                statusProject.Relation = 0;
            }
            _context.StatusProject.Add(statusProject);
            await _context.SaveChangesAsync();
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }

        // PUT: api/Projects/5
        [HttpPut, Authorize(Roles = "Project Manager")]
        [Route("EditStatusProject")]
        public async Task<IActionResult> UpdateStatusProject([FromBody] StatusProjectModel status)
        {
            string str = "";

            foreach (var item in status.statusData)
            {
                var thisStatusProject = await _context.StatusProject.SingleOrDefaultAsync(m => m.Id == item.Id);
                if (item.StatusName == null && item.Serial > 0)
                {
                    thisStatusProject.Serial = item.Serial;
                }
                else if (item.StatusName != null && item.Serial == 0)
                {
                    thisStatusProject.StatusName = item.StatusName;
                }
                else
                {
                    thisStatusProject.Relation = item.Relation;
                }
                _context.StatusProject.Update(thisStatusProject);
            }
            await _context.SaveChangesAsync();
            str = "update successfully";
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }

        // DELETE: api/Templates/5
        [HttpDelete, Authorize(Roles = "Project Manager")]
        [Route("DeleteStatus/{id}")]
        public async Task<IActionResult> DeleteStatus([FromRoute] int id)
        {
            var statusProject = await _context.StatusProject.FirstOrDefaultAsync(m => m.Id == id);

            string str = "Delete Successfully";
            var result = JsonConvert.SerializeObject(new { result = str });
            if (statusProject == null)
            {
                return NotFound();
            }
            else
            {
                _context.StatusProject.Remove(statusProject);

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