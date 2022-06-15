using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MendzerWydatkow_v2.Data;
using MendzerWydatkow_v2.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MendzerWydatkow_v2.Controllers
{
    [Authorize] //dostęp tylko po zalogowaniu
    public class ExpensesController : Controller //klasa główna dziedziczy po klasie controller
    {
        private readonly ApplicationDbContext _context;

        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Akcja Index, czyli Widok dla konkretnego użytkownika, wyszukiwanie, listbox z kategoriami
        public async Task<IActionResult> Index(string ExpCategory, string searchString) //metoda index z paramterami ExpCat i searchString 
        {   
            //Wersyfikacja konkretnego użytkownika
            var claimsIdentity = (ClaimsIdentity)this.User.Identity; //zmienna domniemana 
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier); //find first wyszukaj element o zadanym identyfikatorze
            var userId = claim.Value;
            //

            var expenses = from m in _context.Expense //zmienna dla której nie podajemy typu, kompilator sam domyśla się jakiego jest typu
                           select m;
            
            IQueryable<string> CategoryQuery = from m in _context.Expense
                                            orderby m.Category
                                            select m.Category;

            if (!String.IsNullOrEmpty(searchString)) //jesli szukana fraza nie jest pusta
            {
                expenses = expenses.Where(s => s.Name.Contains(searchString)).Where(s => s.UserId == userId); //LINQ
            }
            else
            {
                expenses = expenses.Where(s => s.UserId == userId); //tworzona jest zmienna expenses i przypisywany jest do niej Id uzytkownika
            }

            // Pasek z kategoriami
            if (!string.IsNullOrEmpty(ExpCategory)) //jesli szukana fraza nie jest pusta
            {
                expenses = expenses.Where(x => x.Category == ExpCategory).Where(s => s.UserId == userId);
            }

            var expenseCategoryVM = new ExpenseCategory //metoda tworzy nową kategorię
            {
                Categories = new SelectList(await CategoryQuery.Distinct().ToListAsync()), //nowa lista 
                Expenses = await expenses.ToListAsync()
            };

            return View(expenseCategoryVM); //zmienna/model expenseCategoryVM ma zostać przekazana do widoku Index

        }
        //Dane dla Raportów
        public async Task<IActionResult> Reports() //metoda reports do raportów
        {   
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            IQueryable<ExpenseReport> data = from expenses in _context.Expense
                                             where expenses.UserId == userId
                                                   group expenses by expenses.Category into CategoryGroup
                                                   select new ExpenseReport()
                                                   {
                                                       Category = CategoryGroup.Key,
                                                       AmountSum = CategoryGroup.Sum(c => c.Amount).ToString()
                                                       
                                                   };
            return View(data.ToList()); //ma zwrócić widok Reports z parametrem data do listy - wyświetlona zostanie lista kategorii z sumą wydatków

        }
        
        //Metoda GET służy do pobierania danych z określonego zasobu
        //Metoda POST służy do wysyłania danych do serwera w celu utworzenia lub aktualizacji zasobów
        // GET: Expenses/Details/5 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expense
                .FirstOrDefaultAsync(m => m.ExpenseId == id); //metoda FirstOrDeaultAsync - wyszukuje w obrębie wydatków element o odpowiednim id
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: Expenses/Create 
        public IActionResult Create() //Metoda uruchamiana w momencie dodawania wydatków 
        {
            return View();
        }

        // POST: Expenses/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpenseId,Name,Description,Amount,Date,Category,UserId")] Expense expense) //typ Expense parametr expense wykorzystano atrybut bind - poczytać o nim (http://www.pzielinski.com/?p=1906)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            if (ModelState.IsValid)
            {
                expense.UserId = userId;
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expense.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }
            return View(expense);
        }

        // POST: Expenses/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpenseId,Name,Description,Amount,Date,Category,UserId")] Expense expense) 
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            if (id != expense.ExpenseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    expense.UserId = userId;
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.ExpenseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expense
                .FirstOrDefaultAsync(m => m.ExpenseId == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expense.FindAsync(id);
            _context.Expense.Remove(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); //po wykonaniu akcji przenieś do okna głównego
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expense.Any(e => e.ExpenseId == id);
        }
    }
}
