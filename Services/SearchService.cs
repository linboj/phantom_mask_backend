using AutoMapper;
using Backend.DTO;
using Backend.Interface;
using Backend.Models;
using Backend.Parameters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class SearchService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public SearchService(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        internal async Task<SearchResultDTO> Search(SearchByNameParameter parameter)
        {
            List<ISearchable> results = new();
            int totalCount = 0;
            var keywordLower = parameter.Keyword.ToLower();

            char[] splitChars = [' ', '.', ',', ';', ':', '-', '!', '?'];
            switch (parameter.Type)
            {
                case SearchType.Pharmacy:
                    var rankedPharmacies = await _dataContext.Pharmacies
                                .Select(p => new
                                {
                                    Pharmacy = p,
                                    // Count occurrences of the keyword in the Name
                                    Relevance = p.Name.ToLower().Split(splitChars, StringSplitOptions.RemoveEmptyEntries)
                                        .Count(word => word.Contains(keywordLower)),
                                    // Find the position of the first occurrence of the keyword
                                    FirstOccurrencePosition = p.Name.ToLower().IndexOf(keywordLower)
                                })
                                .Where(p => p.Relevance > 0) // Only include item with matches
                                .OrderByDescending(p => p.Relevance) // Rank by frequency first
                                .ThenBy(p => p.FirstOccurrencePosition) // Then rank by position of first match
                                .Select(p => p.Pharmacy) // Return the original
                                .ToListAsync();
                    totalCount = rankedPharmacies.Count;
                    rankedPharmacies = rankedPharmacies.Skip(parameter.Offset).Take(parameter.Limit).Select(rp => rp).ToList();
                    results.AddRange(_mapper.Map<List<PharmacyBaseDTO>>(rankedPharmacies));
                    break;
                case SearchType.Mask:
                    var rankedMasks = await _dataContext.Masks
                                .Include(m => m.MaskType)
                                .Select(m => new MaskInfoDTO
                                {
                                    Id = m.Id,
                                    Price = m.Price,
                                    Name = m.MaskType.Name,
                                    Color = m.MaskType.Color,
                                    QuantityPerPack = m.MaskType.Quantity,
                                })
                                .Select(m => new
                                {
                                    Mask = m,
                                    // Count occurrences of the keyword in the Name and Color
                                    Relevance = m.Name.ToLower().Split(splitChars, StringSplitOptions.RemoveEmptyEntries)
                                        .Count(word => word.Contains(keywordLower)) + (m.Color.ToLower().Contains(keywordLower) ? 1 : 0),
                                    // Find the position of the first occurrence of the keyword
                                    FirstOccurrencePosition = m.Name.ToLower().IndexOf(keywordLower)
                                })
                                .Where(m => m.Relevance > 0) // Only include item with matches
                                .OrderByDescending(m => m.Relevance) // Rank by frequency first
                                .ThenBy(m => m.FirstOccurrencePosition) // Then rank by position of first match
                                .Select(m => m.Mask) // Return the original
                                .ToListAsync();
                    totalCount = rankedMasks.Count;
                    rankedMasks = rankedMasks.Skip(parameter.Offset).Take(parameter.Limit).Select(rp => rp).ToList();
                    results.AddRange(rankedMasks);
                    break;
                default:
                    break;
            }

            return new SearchResultDTO
            {
                Results = results,
                Metadata = new PaginationMetaData
                {
                    Total = totalCount,
                    Limit = parameter.Limit,
                    Offset = parameter.Offset,
                },
            };

        }
    }
}