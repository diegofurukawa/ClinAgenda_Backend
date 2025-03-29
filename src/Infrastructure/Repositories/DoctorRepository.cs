using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;
using ClinAgenda.src.Core.Entities;
using ClinAgenda.src.Core.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly MySqlConnection _connection;

        public DoctorRepository(MySqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<IEnumerable<DoctorListDTO>> GetDoctorsAsync(
            string? Doctorname, 
            int? specialtyId, 
            int? statusId, 
            int offset, 
            int itemsPerPage
            )
        {

            var innerJoins = new StringBuilder(@"
                FROM DOCTOR D
                INNER JOIN STATUS S ON D.STATUSID = S.STATUSID
                INNER JOIN DOCTOR_SPECIALTY DSPE ON DSPE.DOCTORID = D.DOCTORID
                WHERE 1 = 1 ");

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(Doctorname))
            {
                innerJoins.Append("AND D.NAME LIKE @DoctorName");
                parameters.Add("Name", $"%{Doctorname}%");
            }

            if (specialtyId.HasValue)
            {
                innerJoins.Append("AND DSPE.SPECIALTYID = @SpecialtyId");
                parameters.Add("SpecialtyId", specialtyId.Value);
            }

            if (statusId.HasValue)
            {
                innerJoins.Append("AND S.ID = @StatusId");
                parameters.Add("StatusId", statusId.Value);
            }

            parameters.Add("LIMIT", itemsPerPage);
            parameters.Add("OFFSET", offset);

            var query = $@"
                SELECT DISTINCT
                    D.doctorid AS ID,
                    D.doctorname AS NAME,
                    S.STATUSID,
                    S.STATUSNAME
                {innerJoins}
                ORDER BY D.doctorid
                LIMIT @Limit OFFSET @Offset";

            return await _connection.QueryAsync<DoctorListDTO>(query.ToString(), parameters);
        }

        public async Task<IEnumerable<SpecialtyDoctorDTO>> GetDoctorSpecialtyAsync(int[] doctorIds)
        {
            var query = @"
                SELECT 
                    DS.DOCTORID AS DOCTORID,
                    SP.SPECIALTYID,
                    SP.SPECIALTYNAME,
                    SP.nSCHEDULEDURATION as SCHEDULEDURATION 
                FROM DOCTOR_SPECIALTY DS
                INNER JOIN SPECIALTY SP ON DS.SPECIALTYID = SP.SPECIALTYID
                WHERE DS.DOCTORID IN @DOCTORIDS";

            var parameters = new { DoctorIds = doctorIds };

            return await _connection.QueryAsync<SpecialtyDoctorDTO>(query, parameters);
        }
        public async Task<int> InsertDoctorAsync(DoctorInsertDTO doctor)
        {
            string query = @"
            INSERT INTO Doctor (DoctorName, StatusId) 
            VALUES (@DoctorName, @StatusId);
            SELECT LAST_INSERT_ID();";
            return await _connection.ExecuteScalarAsync<int>(query, doctor);
        }
        public async Task<IEnumerable<DoctorListDTO>> GetDoctorByIdAsync(int doctorid)
        {
            var queryBase = new StringBuilder(@"
                    FROM DOCTOR D
                LEFT JOIN DOCTOR_SPECIALTY DSPE ON D.DOCTORID = DSPE.DOCTORID
                LEFT JOIN STATUS S ON S.STATUSID = D.STATUSID
                LEFT JOIN SPECIALTY SP ON SP.SPECIALTYID = DSPE.SPECIALTYID
                    WHERE 1 = 1");

            var parameters = new DynamicParameters();

            if (doctorid > 0)
            {
                queryBase.Append(" AND D.DOCTORID = @DoctorId");
                parameters.Add("doctorid", doctorid);
            }

            var dataQuery = $@"
        SELECT DISTINCT
            D.DOCTORID, 
            D.doctorNAME, 
            D.STATUSID, 
            S.STATUSNAME,
            DSPE.SPECIALTYID,
            SP.SPECIALTYNAME
        {queryBase}
        ORDER BY D.DOCTORID";

            var doctors = await _connection.QueryAsync<DoctorListDTO>(dataQuery, parameters);

            return doctors;
        }
        public async Task<bool> UpdateDoctorAsync(DoctorDTO doctor)
        {
            string query = @"
            UPDATE Doctor SET 
                doctorName = @DoctorName,
                StatusId = @StatusId
            WHERE Id = @DoctorId;";
            int rowsAffected = await _connection.ExecuteAsync(query, doctor);
            return rowsAffected > 0;
        }
        public async Task<int> DeleteDoctorByIdAsync(int doctorid)
        {
            string query = "DELETE FROM DOCTOR WHERE DoctorId = @DoctorId";

            var parameters = new { DoctorId = doctorid };

            var rowsAffected = await _connection.ExecuteAsync(query, parameters);

            return rowsAffected;
        }

    }

}