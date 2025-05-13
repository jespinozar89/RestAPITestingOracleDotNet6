

using System.Dynamic;
using MiApiORACLE.Models;
using MiApiORACLE.Models.DTO;

namespace MiApiORACLE.Repository.Interfaces
{
    public interface IReportRepository
    {
        public Task<List<ReportDTO>> GetCustomerById(int id);
        public Task<List<ReportDTO>> GetSumProduct();
        public Task<List<ReportDTO>> GetGroupByJoinFullName();
        public Task<List<ReportDTO>> GetGroupByJoinCountMax();
        public Task<List<ReportDTO>> GetGroupByJoinSum();
        public Task<List<ReportDTO>> GetGroupByLeftJoinHaving();
        public Task<List<ReportDTO>> GetGroupByLeftJoinCount();
        public Task<List<ReportDTO>> GetGroupByJoinBetweenSum();
        public Task<List<ReportDTO>> GetGroupByJoinHavingWhere();
        public Task<List<ReportDTO>> GetGroupByJoinX2HavingWhere();
        public Task<List<ReportDTO>> GetGroupByJoinCountDistinctHavingWhere();
        public Task<List<ReportDTO>> EjercicioCO2_N1();
        public Task<List<ReportDTO>> EjercicioCO2_N2();
        public Task<List<ReportDTO>> EjercicioCO2_N3();
        public Task<List<ReportDTO>> EjercicioCO2_N4();
        public Task<List<ReportDTO>> EjercicioCO2_N5();
        public Task<List<ReportDTO>> EjercicioCO2_N6();
        public Task<List<ReportDTO>> EjercicioCO2_N7();
        public Task<List<ReportDTO>> EjercicioCO2_N8();
        public Task<List<ReportDTO>> EjercicioCO2_N9();
        public Task<List<ReportDTO>> EjercicioCO2_N10();

    }
}