using System.Data.Common;
using MiApiORACLE.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MiApiORACLE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;
        public ReportController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [HttpGet("GetCustomerById/{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                var report = await _reportRepository.GetCustomerById(id);
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetSumProduct")]
        public async Task<IActionResult> GetSumProduct()
        {
            try
            {
                var report = await _reportRepository.GetSumProduct();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetGroupByJoinFullName")]
        public async Task<IActionResult> GetGroupByJoinFullName()
        {
            try
            {
                var report = await _reportRepository.GetGroupByJoinFullName();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetGroupByJoinCountMax")]
        public async Task<IActionResult> GetGroupByJoinCountMax()
        {
            try
            {
                var report = await _reportRepository.GetGroupByJoinCountMax();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetGroupByJoinSum")]
        public async Task<IActionResult> GetGroupByJoinSum()
        {
            try
            {
                var report = await _reportRepository.GetGroupByJoinSum();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetGroupByLeftJoinHaving")]
        public async Task<IActionResult> GetGroupByLeftJoinHaving()
        {
            try
            {
                var report = await _reportRepository.GetGroupByLeftJoinHaving();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetGroupByLeftJoinCount")]
        public async Task<IActionResult> GetGroupByLeftJoinCount()
        {
            try
            {
                var report = await _reportRepository.GetGroupByLeftJoinCount();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetGroupByJoinBetweenSum")]
        public async Task<IActionResult> GetGroupByJoinBetweenSum()
        {
            try
            {
                var report = await _reportRepository.GetGroupByJoinBetweenSum();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetGroupByJoinHavingWhere")]
        public async Task<IActionResult> GetGroupByJoinHavingWhere()
        {
            try
            {
                var report = await _reportRepository.GetGroupByJoinHavingWhere();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetGroupByJoinX2HavingWhere")]
        public async Task<IActionResult> GetGroupByJoinX2HavingWhere()
        {
            try
            {
                var report = await _reportRepository.GetGroupByJoinX2HavingWhere();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetGroupByJoinCountDistinctHavingWhere")]
        public async Task<IActionResult> GetGroupByJoinCountDistinctHavingWhere()
        {
            try
            {
                var report = await _reportRepository.GetGroupByJoinCountDistinctHavingWhere();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("EjercicioCO2_N1")]
        public async Task<IActionResult> EjercicioCO2_N1()
        {
            try
            {
                var report = await _reportRepository.EjercicioCO2_N1();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("EjercicioCO2_N2")]
        public async Task<IActionResult> EjercicioCO2_N2()
        {
            try
            {
                var report = await _reportRepository.EjercicioCO2_N2();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("EjercicioCO2_N3")]
        public async Task<IActionResult> EjercicioCO2_N3()
        {
            try
            {
                var report = await _reportRepository.EjercicioCO2_N3();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("EjercicioCO2_N4")]
        public async Task<IActionResult> EjercicioCO2_N4()
        {
            try
            {
                var report = await _reportRepository.EjercicioCO2_N4();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("EjercicioCO2_N5")]
        public async Task<IActionResult> EjercicioCO2_N5()
        {
            try
            {
                var report = await _reportRepository.EjercicioCO2_N5();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("EjercicioCO2_N6")]
        public async Task<IActionResult> EjercicioCO2_N6()
        {
            try
            {
                var report = await _reportRepository.EjercicioCO2_N6();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("EjercicioCO2_N7")]
        public async Task<IActionResult> EjercicioCO2_N7()
        {
            try
            {
                var report = await _reportRepository.EjercicioCO2_N7();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("EjercicioCO2_N8")]
        public async Task<IActionResult> EjercicioCO2_N8()
        {
            try
            {
                var report = await _reportRepository.EjercicioCO2_N8();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("EjercicioCO2_N9")]
        public async Task<IActionResult> EjercicioCO2_N9()
        {
            try
            {
                var report = await _reportRepository.EjercicioCO2_N9();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException ex)
            {
                // Log the exception
                return StatusCode(500, "Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("EjercicioCO2_N10")]
        public async Task<IActionResult> EjercicioCO2_N10()
        {
            try
            {
                var report = await _reportRepository.EjercicioCO2_N10();
                if (report == null || !report.Any())
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (DbException ex)
            {
                // Log the exception
                return StatusCode(500, "Database error: " + ex.Message);
            }
            catch (Exception ex) 
            {
                // Log the exception
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}