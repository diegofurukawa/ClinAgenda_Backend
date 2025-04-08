.
├── appsettings.Development.json
├── appsettings.json
├── ClinAgenda.csproj
├── ClinAgenda.http
├── ClinAgenda.sln
├── LICENSE
├── Program.cs
├── Properties
│   └── launchSettings.json
├── README.md
├── scripts
│   └── search.sh
├── sql
│   ├── create_datebase.sql
│   └── create_user_admin.sql
├── src
│   ├── Application
│   │   ├── DTOs
│   │   │   ├── Appointment
│   │   │   │   ├── AppointmentDTO.cs
│   │   │   │   ├── AppointmentInsertDTO.cs
│   │   │   │   ├── AppointmentListDTO.cs
│   │   │   │   └── AppointmentListReturnDTO.cs
│   │   │   ├── Auth
│   │   │   │   ├── AuthResponseDTO.cs
│   │   │   │   ├── UserLoginDTO.cs
│   │   │   │   └── UserRegistrationDTO.cs
│   │   │   ├── Doctor
│   │   │   │   ├── DoctorCreateDTO.cs
│   │   │   │   ├── DoctorDetailDTO.cs
│   │   │   │   ├── DoctorDTO.cs
│   │   │   │   ├── DoctorInsertDTO.cs
│   │   │   │   ├── DoctorListDTO.cs
│   │   │   │   ├── DoctorListItemDTO.cs
│   │   │   │   ├── DoctorPagedResultDTO.cs
│   │   │   │   ├── DoctorResponseDTO.cs
│   │   │   │   ├── DoctorSpecialtyDetailDTO.cs
│   │   │   │   ├── DoctorSpecialtyDTO.cs
│   │   │   │   ├── DoctorSpecialtyResponseDTO.cs
│   │   │   │   ├── DoctorToggleActiveDTO.cs
│   │   │   │   ├── DoctorUpdateDTO.cs
│   │   │   │   └── DoctorWithStatusDTO.cs
│   │   │   ├── Patient
│   │   │   │   ├── PatientDTO.cs
│   │   │   │   ├── PatientInsertDTO.cs
│   │   │   │   ├── PatientListDTO.cs
│   │   │   │   └── PatientListReturnDTO.cs
│   │   │   ├── Specialty
│   │   │   │   ├── SpecialtyDTO.cs
│   │   │   │   ├── SpecialtyInsertDTO.cs
│   │   │   │   └── SpecialtyResponseDTO.cs
│   │   │   ├── Status
│   │   │   │   ├── StatusDTO.cs
│   │   │   │   ├── StatusInsertDTO.cs
│   │   │   │   └── StatusResponseDTO.cs
│   │   │   └── User
│   │   │       └── UserDTO.cs
│   │   ├── Services
│   │   │   └── JwtService.cs
│   │   └── UseCases
│   │       ├── AppointmentUseCase.cs
│   │       ├── AuthUseCase.cs
│   │       ├── DoctorUseCase.cs
│   │       ├── PatientUseCase.cs
│   │       ├── SpecialtyUseCase.cs
│   │       └── StatusUseCase.cs
│   ├── Core
│   │   ├── Entities
│   │   │   ├── Appointment.cs
│   │   │   ├── AuthToken.cs
│   │   │   ├── Doctor.cs
│   │   │   ├── DoctorSpecialty.cs
│   │   │   ├── Patient.cs
│   │   │   ├── Role.cs
│   │   │   ├── Specialty.cs
│   │   │   ├── Status.cs
│   │   │   ├── Test.cs
│   │   │   ├── User.cs
│   │   │   └── UserRole.cs
│   │   ├── Enums
│   │   │   └── StatusType.cs
│   │   └── Interfaces
│   │       ├── IAppointmentRepository.cs
│   │       ├── IAuthRepository.cs
│   │       ├── IDoctorRepository.cs
│   │       ├── IDoctorSpecialtyRepository.cs
│   │       ├── IPatientRepository.cs
│   │       ├── IRoleRepository.cs
│   │       ├── ISpecialtyRepository.cs
│   │       ├── IStatusRepository.cs
│   │       ├── IUserEntityRepository.cs
│   │       └── IUserRepository.cs
│   ├── Infrastructure
│   │   └── Repositories
│   │       ├── AppointmentRepository.cs
│   │       ├── AuthRepository.cs
│   │       ├── DoctorRepository.cs
│   │       ├── DoctorSpecialtyRepository.cs
│   │       ├── PatientRepository.cs
│   │       ├── RoleRepository.cs
│   │       ├── SpecialtyRepository.cs
│   │       ├── StatusRepository.cs
│   │       ├── UserEntityRepository.cs
│   │       └── UserRepository.cs
│   └── WebAPI
│       ├── Controllers
│       │   ├── AppointmentController.cs
│       │   ├── AuthController.cs
│       │   ├── DoctorController.cs
│       │   ├── PatientController.cs
│       │   ├── SpecialtyController.cs
│       │   └── StatusController.cs
│       └── Filters
│           └── EntityAuthorizationAttribute.cs
└── swagger.json

24 directories, 91 files