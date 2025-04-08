using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Doctor;
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
            string? doctorName, 
            int? specialtyId, 
            int? statusId,
            bool? isActive,
            int offset, 
            int pageSize)
        {
            var innerJoins = new StringBuilder(@"
                from doctor d
                inner join status s on d.statusid = s.statusid
                inner join doctor_specialty dspe on dspe.doctorid = d.doctorid
                where 1 = 1 ");

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(doctorName))
            {
                innerJoins.Append(" AND d.doctorName LIKE @doctorName");
                parameters.Add("doctorName", $"%{doctorName}%");
            }

            if (specialtyId.HasValue)
            {
                innerJoins.Append(" AND dspe.SPECIALTYID = @SpecialtyId");
                parameters.Add("SpecialtyId", specialtyId.Value);
            }

            if (statusId.HasValue)
            {
                innerJoins.Append(" AND s.StatusId = @StatusId");
                parameters.Add("StatusId", statusId.Value);
            }
            
            if (isActive.HasValue)
            {
                innerJoins.Append(" AND d.lActive = @IsActive");
                parameters.Add("IsActive", isActive.Value);
            }

            parameters.Add("Limit", pageSize);
            parameters.Add("Offset", offset);

            var query = $@"
                select distinct
                    d.doctorid,
                    d.doctorname,
                    s.statusid,
                    s.statusname,
                    d.dcreated as dcreated,
                    d.dlastupdated as dlastupdated,
                    d.lactive as lactive
                {innerJoins}
                ORDER BY d.doctorid
                LIMIT @Limit OFFSET @Offset";

            return await _connection.QueryAsync<DoctorListDTO>(query.ToString(), parameters);
        }

        public async Task<IEnumerable<DoctorSpecialtyDTO>> GetDoctorSpecialtyAsync(int[] doctorIds)
        {
            var query = @"
                select 
                    ds.doctorid,
                    sp.specialtyid,
                    sp.specialtyname,
                    sp.nscheduleduration,
                    sp.dcreated,
                    sp.dlastupdated,
                    sp.lactive
                from doctor_specialty ds
                inner join specialty sp on ds.specialtyid = sp.specialtyid
                where ds.doctorid in @doctorids";

            var parameters = new { DoctorIds = doctorIds };

            return await _connection.QueryAsync<DoctorSpecialtyDTO>(query, parameters);
        }
        
        public async Task<int> InsertDoctorAsync(DoctorInsertDTO doctor)
        {
            string query = @"
            insert into doctor (
                doctorname, 
                statusid, 
                dcreated, 
                lactive
            ) 
            VALUES (
                @DoctorName, 
                @StatusId, 
                NOW(), 
                @LActive
            );
            SELECT LAST_INSERT_ID();";
            
            int newDoctorId = await _connection.ExecuteScalarAsync<int>(query, doctor);

            // Inserir especialidades para o novo médico
            if (doctor.Specialty != null && doctor.Specialty.Count > 0)
            {
                var specialtyInserts = new List<dynamic>();
                foreach (var specialtyId in doctor.Specialty)
                {
                    specialtyInserts.Add(new
                    {
                        DoctorId = newDoctorId,
                        SpecialtyId = specialtyId,
                        LActive = true
                    });
                }

                string specialtyQuery = @"
                insert into doctor_specialty (
                    doctorid,
                    specialtyid,
                    dcreated,
                    lactive
                ) values (
                    @DoctorId,
                    @SpecialtyId,
                    NOW(),
                    @LActive
                )";

                await _connection.ExecuteAsync(specialtyQuery, specialtyInserts);
            }

            return newDoctorId;
        }
        
        public async Task<IEnumerable<DoctorListDTO>> GetDoctorByIdAsync(int doctorid)
        {
            var queryBase = new StringBuilder(@"
                    from doctor d
                left join doctor_specialty dspe on d.doctorid = dspe.doctorid
                left join status s on s.statusid = d.statusid
                left join specialty sp on sp.specialtyid = dspe.specialtyid
                    where 1 = 1");

            var parameters = new DynamicParameters();

            if (doctorid > 0)
            {
                queryBase.Append(" AND d.doctorId = @DoctorId");
                parameters.Add("DoctorId", doctorid);
            }

            var dataQuery = $@"
        select distinct
            d.doctorid, 
            d.doctorname, 
            d.statusid, 
            s.statusname,
            dspe.specialtyid,
            sp.specialtyname,
            sp.nscheduleduration,
            d.dcreated,
            d.dlastupdated,
            d.lactive
        {queryBase}
        ORDER BY d.doctorid";

            var doctors = await _connection.QueryAsync<DoctorListDTO>(dataQuery, parameters);

            return doctors;
        }
        
        public async Task<bool> UpdateDoctorAsync(int doctorid, DoctorInsertDTO doctor)
        {
            // Atualizar dados básicos do médico
            string query = @"
            update doctor set 
                doctorname = @doctorname,
                statusid = @statusid,
                dlastupdated = now(),
                lactive = @LActive
            WHERE DoctorId = @DoctorId";
            
            var parameters = new DynamicParameters(doctor);
            parameters.Add("DoctorId", doctorid);
            
            int rowsAffected = await _connection.ExecuteAsync(query, parameters);
            
            if (rowsAffected <= 0)
                return false;

            // Remover especialidades existentes
            await _connection.ExecuteAsync(
                "delete from doctor_specialty where doctorid = @DoctorId", 
                new { DoctorId = doctorid });

            // Adicionar novas especialidades
            if (doctor.Specialty != null && doctor.Specialty.Count > 0)
            {
                var specialtyInserts = new List<dynamic>();
                foreach (var specialtyId in doctor.Specialty)
                {
                    specialtyInserts.Add(new
                    {
                        DoctorId = doctorid,
                        SpecialtyId = specialtyId,
                        LActive = true
                    });
                }

                string specialtyQuery = @"
                insert into doctor_specialty (
                    doctorid,
                    specialtyid,
                    dcreated,
                    lactive
                ) values (
                    @DoctorId,
                    @SpecialtyId,
                    NOW(),
                    @LActive
                )";

                await _connection.ExecuteAsync(specialtyQuery, specialtyInserts);
            }
            
            return true;
        }
        
        public async Task<int> DeleteDoctorByIdAsync(int doctorid)
        {
            // Primeiro excluir as relações de especialidade
            await _connection.ExecuteAsync(
                "delete from doctor_specialty where doctorid = @DoctorId", 
                new { DoctorId = doctorid });
                
            // Então excluir o médico
            string query = "delete from doctor where doctorid = @DoctorId";
            var parameters = new { DoctorId = doctorid };
            return await _connection.ExecuteAsync(query, parameters);
        }
        
        public async Task<int> ToggleDoctorActiveAsync(int doctorid, bool active)
        {
            string query = @"
            UPDATE doctor SET 
                lActive = @Active,
                dlastupdated = NOW()
            WHERE DoctorId = @DoctorId";

            var parameters = new { DoctorId = doctorid, Active = active };
            return await _connection.ExecuteAsync(query, parameters);
        }
    }
}