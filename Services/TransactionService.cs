using AutoMapper;
using Backend.DTO;
using Backend.Parameters;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Exceptions;

namespace Backend.Services
{
    public class TransactionService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public TransactionService(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        internal async Task<Transaction> Create(TransactionCreateDto transaction)
        {
            var mask = await _dataContext.Masks
                        .Where(mask => mask.Id == transaction.MaskId)
                        .Include(mask => mask.Pharmacy)
                        .SingleOrDefaultAsync();
            decimal totalAmount = 0;
            if (mask == null)
                throw new TransactionCreateValidationException([$"Mask({transaction.MaskId}) isn't found "]);
            else
                totalAmount = mask.Price * transaction.Quantity;
            var user = await _dataContext.Users
                        .Where(user => user.Id == transaction.UserId)
                        .SingleOrDefaultAsync();
            if (user == null)
                throw new TransactionCreateValidationException([$"User({transaction.UserId}) isn't found"]);
            else
            {
                if (user.CashBalance < totalAmount)
                    throw new TransactionCreateValidationException([$"User({transaction.UserId}) has Insufficient cash"]);
            }

            var pharmacy = mask.Pharmacy;
            pharmacy.CashBalance += totalAmount;
            user.CashBalance -= totalAmount;
            _dataContext.Pharmacies.Add(pharmacy);
            _dataContext.Users.Add(user);

            Transaction record = new()
            {
                UserId = transaction.UserId,
                MaskId = transaction.MaskId,
                PharmacyId = pharmacy.Id,
                TransactionAmount = totalAmount,
            };
            _dataContext.Transactions.Add(record);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return record;
        }

        internal async Task<TransactionStatisticsDTO> GetStatistics(InDateRangeQueryParameter parameter)
        {
            DateTime StartDate = parameter.StartDate.ToDateTime(TimeOnly.MinValue);
            DateTime EndDate = parameter.EndDate.ToDateTime(TimeOnly.MaxValue);
            var transactions = await _dataContext.Transactions
                                    .Where(t => t.TransactionDate >= StartDate && t.TransactionDate <= EndDate)
                                    .ToListAsync();
            TransactionStatisticsDTO result = new TransactionStatisticsDTO
            {
                TotalAmount = transactions.Sum(t => t.TransactionAmount),
                TotalNumber = transactions.Count
            };

            return result;
        }

        internal async Task<List<UserWithTotalTranscationAmountDTO>> GetTopUser(TopUsersQueryParameter parameter)
        {
            try
            {

                DateTime StartDate = parameter.StartDate.ToDateTime(TimeOnly.MinValue);
                DateTime EndDate = parameter.EndDate.ToDateTime(TimeOnly.MaxValue);
                var users = await _dataContext.Transactions
                                .Where(t => t.TransactionDate >= StartDate && t.TransactionDate <= EndDate)
                                .Include(t => t.User)
                                .GroupBy(t => t.User)
                                .Select(g => new UserWithTotalTranscationAmountDTO
                                {
                                    Id = g.Key.Id,
                                    Name = g.Key.Name,
                                    TotalTranscationAmount = g.Sum(t => t.TransactionAmount)
                                })
                                .OrderByDescending(user => user.TotalTranscationAmount)
                                .Take(parameter.Limit)
                                .ToListAsync();
                return users;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex);
                throw;
            }
        }

        internal async Task<TransactionGetDTO?> GetTransition(Guid id)
        {
            var transaction = await _dataContext.Transactions
                                .Where(t => t.Id == id)
                                .Include(t => t.User)
                                .Include(t => t.Pharmacy)
                                .Include(t => t.Mask)
                                .ThenInclude(t => t.MaskType)
                                .SingleOrDefaultAsync();
            if (transaction != null) return _mapper.Map<TransactionGetDTO>(transaction);
            return null;
        }
    }

}