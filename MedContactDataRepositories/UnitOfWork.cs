﻿using MedContactDb;
using MedContactDb.Entities;
using MedContactDataAbstractions;
using MedContactDataAbstractions.Repositories;

namespace MedContactDataRepositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MedContactContext _database;
        public IBaseUserRepository<Customer> CustomerRepository { get; }
        public IBaseUserRepository<User> UserRepository { get; }
        public IBaseUserRepository<Doctor> DoctorRepository { get; }


        public UnitOfWork(MedContactContext database, IBaseUserRepository<Customer> customerRepository,
               IBaseUserRepository<User> userRepository, IBaseUserRepository<Doctor> doctorRepository)
        {
            _database = database;
            CustomerRepository = customerRepository;
            UserRepository = userRepository;
            DoctorRepository = doctorRepository;
        }

        public async Task<int> Commit()
        {
            return await _database.SaveChangesAsync();
        }
    }
}