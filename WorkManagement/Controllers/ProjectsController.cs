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
    public class project
    {
        public Project[] projectData;
    }
    public class listUser
    {
        public ListUserInProject[] listUserData;
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
            foreach (var item in project)
            {
                if(item.User_id == UserId)
                {
                    var data = new { id = item.Id, name = item.Name, description = item.Description, status = item.Status,progress = 100 };
                    str += JsonConvert.SerializeObject(data) + ",";
                }
            }
            str = str.Remove(str.Length - 1);
            str = "[" + str + "]";
            if (project == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(str);

            }
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
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
                thisProject.Name = item.Name;
                thisProject.Description = item.Description;
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
        [Route("AddProject"),Authorize(Roles = "Project Manager")]
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
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }

        // POST: api/Projects
        [HttpPost]
        [Route("AddListUser"), Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> AddListUser([FromBody] listUser listUser)
        {
            string str = "Add member successfully";
            ListUserInProject listUserProject = new ListUserInProject();
            foreach (var item in listUser.listUserData)
            {
                listUserProject.Project_Id = item.Project_Id;
                listUserProject.User_id = item.User_id;
            }
            _context.ListUserInProject.Add(listUserProject);
            await _context.SaveChangesAsync();
            var result = JsonConvert.SerializeObject(new { result = str });
            return Ok(result);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] int id)
        {
            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Project.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }
    }
}