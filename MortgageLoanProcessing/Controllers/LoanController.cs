using Microsoft.AspNetCore.Mvc;
using MortgageLoanProcessing.Repositories;
using MortgageLoanProcessing.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MortgageLoanProcessing.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MortgageLoanProcessing.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]


    public class LoanController : ControllerBase
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IInterestRateRepository _interestRateRepository;
        private readonly IAmortizationRepository _amortizationRepository;
        private readonly UserManager<User> _userManager;

        public LoanController(ILoanRepository loanRepository, IInterestRateRepository interestRateRepository, IAmortizationRepository amortizationRepository, UserManager<User> userManager)
        {
            _loanRepository = loanRepository;
            _interestRateRepository = interestRateRepository;
            _amortizationRepository = amortizationRepository;
            _userManager = userManager;
        }

        [HttpGet]
        

        public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoans()
        {
            var loans = await _loanRepository.GetAllLoansAsync();
            var loanDtos = loans.Select(l => new LoanDto
            {
                Id = l.Id,
                LoanAmount = l.LoanAmount,
                LoanTermYears = l.LoanTermYears,
                InterestRate = l.InterestRate,
                InterestRateId = l.InterestRateId,
                ApplicationDate = l.ApplicationDate
            });

            return Ok(loanDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LoanDto>> GetLoan(int id)
        {
            var loan = await _loanRepository.GetLoanByIdAsync(id);
            if (loan == null) return NotFound();

            return new LoanDto
            {
                Id = loan.Id,
                LoanAmount = loan.LoanAmount,
                LoanTermYears = loan.LoanTermYears,
                InterestRate = loan.InterestRate,
                InterestRateId = loan.InterestRateId,
                ApplicationDate = loan.ApplicationDate
            };
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<LoanDto>> CreateLoan(LoanDto loanDto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Username not found in token.");
            }

            // Fetch User ID from the database using the username
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized("User not found in the database.");
            }

            var userId = user.Id;



            // Validate interest rate selection
            var selectedInterestRate = await _interestRateRepository.GetInterestRateByIdAsync(loanDto.InterestRateId);
            if (selectedInterestRate == null)
            {
                return BadRequest("Invalid Interest Rate selection");
            }

            var loan = new Loan
            {
                LoanAmount = loanDto.LoanAmount,
                LoanTermYears = loanDto.LoanTermYears,
                InterestRate = selectedInterestRate.Rate,  // Assign the actual rate
                ApplicationDate = DateTime.UtcNow,
                InterestRateId = loanDto.InterestRateId,
                UserId = userId,
            };

            await _loanRepository.AddLoanAsync(loan);


            return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, new LoanDto
            {
                Id = loan.Id,
                LoanAmount = loan.LoanAmount,
                LoanTermYears = loan.LoanTermYears,
                InterestRate = loan.InterestRate,
                InterestRateId = loanDto.InterestRateId,
                ApplicationDate = loan.ApplicationDate,
                 //UserId = userId,
            });

        }


        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateLoan(int id, LoanDto loanDto)
        //{
        //    var existingLoan = await _loanRepository.GetLoanByIdAsync(id);
        //    if (existingLoan == null) return NotFound();

        //    existingLoan.LoanAmount = loanDto.LoanAmount;
        //    existingLoan.LoanTermYears = loanDto.LoanTermYears;
        //    existingLoan.InterestRate = loanDto.InterestRate;
        //    existingLoan.ApplicationDate = loanDto.ApplicationDate;

        //    await _loanRepository.UpdateLoanAsync(existingLoan);

        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteLoan(int id)
        //{
        //    await _loanRepository.DeleteLoanAsync(id);
        //    return NoContent();
        //}
        [HttpGet("interest-rates")]
        public async Task<ActionResult<IEnumerable<InterestRate>>> GetInterestRates()
        {
            var interestRates = await _interestRateRepository.GetAllInterestRatesAsync();
            return Ok(interestRates);
        }

        [HttpGet("{loanId}/amortization-schedule")]
        public async Task<ActionResult<IEnumerable<AmortizationScheduleDto>>> GetAmortizationSchedule(
        int loanId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var loan = await _loanRepository.GetLoanByIdAsync(loanId);
            if (loan == null) return NotFound("Loan not found");

            var totalRecords = await _amortizationRepository.GetTotalCountByLoanIdAsync(loanId);
            var amortizationSchedule = await _amortizationRepository.GetScheduleByLoanIdAsync(loanId, pageNumber, pageSize);
            var TotalPages = totalRecords > 0 ? (int)Math.Ceiling(totalRecords / (double)pageSize) : 1;

            var scheduleDtos = amortizationSchedule.Select(a => new AmortizationScheduleDto
            {
                PaymentNumber = a.PaymentNumber,
                PaymentDate = a.PaymentDate,
                PrincipalPayment = a.PrincipalPayment,
                InterestPayment = a.InterestPayment,
                RemainingBalance = a.RemainingBalance
            });

            var response = new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Data = scheduleDtos
            };

            return Ok(response);
        }

        [HttpGet("user-loans")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetUserLoans()
        {
            //  Get the username from the token
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Username not found in token.");
            }

            Console.WriteLine($"Username from token: {username}");

            //  Retrieve user details from the database
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                Console.WriteLine($"User not found in database for username: {username}");
                return Unauthorized("User not found in the database.");
            }

            var userId = user.Id;
            Console.WriteLine($"User ID retrieved: {userId}");

            //  Fetch loans associated with the user
            var loans = await _loanRepository.GetLoansByUserIdAsync(userId);
            if (!loans.Any())
            {
                return NotFound("No loans found for the user.");
            }

            // Convert to DTOs
            var loanDtos = loans.Select(l => new LoanDto
            {
                Id = l.Id,
                LoanAmount = l.LoanAmount,
                LoanTermYears = l.LoanTermYears,
                InterestRate = l.InterestRate,
                InterestRateId = l.InterestRateId,
                ApplicationDate = l.ApplicationDate
            }).ToList();

            foreach (var loan in loanDtos)
            {
                Console.WriteLine($"Loan ID: {loan.Id}, Amount: {loan.LoanAmount}, Interest: {loan.InterestRate}");
            }

            return Ok(loanDtos);
        }


        [HttpGet("dashboard/loan-summary/{loanId}")]
        public async Task<ActionResult<LoanDto>> GetLoanSummary(int loanId)
        {
            var loan = await _loanRepository.GetLoanByIdAsync(loanId);
            if (loan == null) return NotFound("Loan not found");

            var loanDto = new LoanDto
            {
                Id = loan.Id,
                LoanAmount = loan.LoanAmount,
                LoanTermYears = loan.LoanTermYears,
                InterestRate = loan.InterestRate,
                InterestRateId = loan.InterestRateId,
                ApplicationDate = loan.ApplicationDate

            };

            return Ok(loanDto);
        }

        [HttpGet("dashboard/amortization-data/{loanId}")]
        public async Task<ActionResult<IEnumerable<AmortizationScheduleDto>>> GetAmortizationData(int loanId)
        {
            var loan = await _loanRepository.GetLoanByIdAsync(loanId);
            if (loan == null) return NotFound("Loan not found");

            var amortizationSchedule = await _amortizationRepository.GetScheduleByLoanIdAsync(loanId, 1, int.MaxValue);

            var scheduleDtos = amortizationSchedule.Select(a => new AmortizationScheduleDto
            {
                PaymentNumber = a.PaymentNumber,
                PaymentDate = a.PaymentDate,
                PrincipalPayment = a.PrincipalPayment,
                InterestPayment = a.InterestPayment,
                RemainingBalance = a.RemainingBalance
            });

            return Ok(scheduleDtos);
        }

        [HttpPost("{loanId}/generate-amortization-schedule")]
        public async Task<ActionResult<IEnumerable<AmortizationScheduleDto>>> GenerateAndSaveAmortizationSchedule(int loanId)
        {
            var loan = await _loanRepository.GetLoanByIdAsync(loanId);
            if (loan == null) return NotFound("Loan not found");

            // Generate the amortization schedule dynamically
            var amortizationSchedule = GenerateAmortizationSchedule(loan);

            // Save to the database (if needed)
            await _amortizationRepository.SaveAmortizationScheduleAsync(amortizationSchedule);

            var scheduleDtos = amortizationSchedule.Select(a => new AmortizationScheduleDto
            {
                PaymentNumber = a.PaymentNumber,
                PaymentDate = a.PaymentDate,
                PrincipalPayment = a.PrincipalPayment,
                InterestPayment = a.InterestPayment,
                RemainingBalance = a.RemainingBalance
            });

            return Ok(scheduleDtos);
        }


        private List<AmortizationSchedule> GenerateAmortizationSchedule(Loan loan)
        {
            List<AmortizationSchedule> schedule = new List<AmortizationSchedule>();

            double monthlyInterestRate =(double)(loan.InterestRate / 100) / 12;
            int totalPayments =  loan.LoanTermYears * 12;
            double loanAmount = (double)loan.LoanAmount;
            double monthlyPayment = (loanAmount * monthlyInterestRate) /
                                    (1 - Math.Pow(1 + monthlyInterestRate, -totalPayments));

            double remainingBalance = loanAmount;
            DateTime paymentDate = DateTime.Today;

            for (int i = 1; i <= totalPayments; i++)
            {
                double interestPayment = remainingBalance * monthlyInterestRate;
                double principalPayment = monthlyPayment - interestPayment;
                remainingBalance -= principalPayment;

                schedule.Add(new AmortizationSchedule
                {
                    LoanId = loan.Id,
                    PaymentNumber = i,
                    PaymentDate = paymentDate,
                    PrincipalPayment = (decimal)principalPayment,
                    InterestPayment = (decimal)interestPayment,
                    RemainingBalance = (decimal)Math.Max(remainingBalance, 0) // Ensure it doesn't go negative
                });

                paymentDate = paymentDate.AddMonths(1);
            }

            return schedule;
        }

    }
}
