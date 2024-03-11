using System;
using System.Collections.Generic;
using System.Linq;

// Клас, що представляє працівника
class Employee
{
    public string Name { get; set; }
    public string Position { get; set; }
    public string Qualification { get; set; }
    public double BaseSalary { get; set; }
    public double TotalHoursWorked { get; set; }
    public double OvertimeHoursWorked { get; set; }
    public double Bonus { get; set; }
}

// Інтерфейс, що визначає методи для нарахування заробітної плати
interface ISalaryCalculator
{
    double CalculateSalary(Employee employee);
}

// Конкретний клас, що реалізує розрахунок заробітної плати для працівника зі штатною зайнятістю
class PermanentEmployeeSalaryCalculator : ISalaryCalculator
{
    public double CalculateSalary(Employee employee)
    {
        return employee.BaseSalary;
    }


}


// Декоратор, який розширює функціонал базового розрахунку заробітної плати
abstract class SalaryDecorator : ISalaryCalculator
{
    protected ISalaryCalculator _calculator;

    public SalaryDecorator(ISalaryCalculator calculator)
    {
        _calculator = calculator;
    }

    public virtual double CalculateSalary(Employee employee)
    {
        return _calculator.CalculateSalary(employee);
    }
}

// Конкретний декоратор, який додає премію до заробітної плати
class BonusDecorator : SalaryDecorator
{
    public BonusDecorator(ISalaryCalculator calculator) : base(calculator) { }

    public override double CalculateSalary(Employee employee)
    {
        double baseSalary = base.CalculateSalary(employee);
        return baseSalary + employee.Bonus;
    }
}

// Фасад для легкого доступу до функціоналу програми
class PayrollSystemFacade
{
    private readonly ISalaryCalculator _calculator;

    public PayrollSystemFacade(ISalaryCalculator calculator)
    {
        _calculator = calculator;
    }

    public double CalculateSalary(Employee employee)
    {
        return _calculator.CalculateSalary(employee);
    }

    public List<Employee> GetTopPerformers(List<Employee> employees, int count)
    {
        return employees.OrderByDescending(e => e.TotalHoursWorked - e.OvertimeHoursWorked).Take(count).ToList();
    }

    public void AwardBonus(Employee employee, double Bonus)
    {
        employee.Bonus += Bonus;
    }

    public List<Employee> GetTopPerformersByHoursWorked(List<Employee> employees, int count)
    {
        return employees.OrderByDescending(e => e.TotalHoursWorked - e.OvertimeHoursWorked).Take(count).ToList();
    }

    public List<Employee> GetTopPerformersBySalary(List<Employee> employees, int count)
    {
        return employees.OrderByDescending(e => CalculateSalary(e)).Take(count).ToList();
    }


}


    class Program
    {
        static void Main(string[] args)
        {
            // Створення працівників
            List<Employee> employees = new List<Employee>
        {
            new Employee { Name = "Олесь", Position = "Software Developer", Qualification = "Senior", BaseSalary = 5000, Bonus = 0, TotalHoursWorked = 160, OvertimeHoursWorked = 10 },
            new Employee { Name = "Мар'яна", Position = "Project Manager", Qualification = "Middle", BaseSalary = 6000, Bonus = 0, TotalHoursWorked = 150, OvertimeHoursWorked = 5 },
            new Employee { Name = "Андрій", Position = "Quality Assurance", Qualification = "Junior", BaseSalary = 4000, Bonus = 0, TotalHoursWorked = 155, OvertimeHoursWorked = 8 }
        };

            // Створення фасаду для доступу до функціоналу програми
            ISalaryCalculator calculator = new PermanentEmployeeSalaryCalculator();
            calculator = new BonusDecorator(calculator);
            PayrollSystemFacade facade = new PayrollSystemFacade(calculator);

            // Використання фасаду для розрахунку заробітної плати працівників
            foreach (var employee in employees)
            {
                double totalSalary = facade.CalculateSalary(employee);
                Console.WriteLine($"Загальна зарплата для {employee.Name}: {totalSalary}");
            }

            // Використання фасаду для визначення найкращих працівників за кількістю відпрацьованих годин
            List<Employee> topPerformersByHours = facade.GetTopPerformersByHoursWorked(employees, 2);
            Console.WriteLine("\nНайкращі працівники за кількістю відпрацьованих годин:");
            foreach (var performer in topPerformersByHours)
            {
                Console.WriteLine($"Ім'я: {performer.Name}, Посада: {performer.Position}, Відпрацьовані години: {performer.TotalHoursWorked}");
            }

            // Використання фасаду для визначення найкращих працівників за зарплатою
            List<Employee> topPerformersBySalary = facade.GetTopPerformersBySalary(employees, 2);
            Console.WriteLine("\nТоп працівників за зарплатою:");
            foreach (var performer in topPerformersBySalary)
            {
                Console.WriteLine($"Ім'я: {performer.Name}, Посада: {performer.Position}, Зарплата: {facade.CalculateSalary(performer)}");
            }

        // Використання фасаду для визначення найкращих працівників та призначення їм премій
        facade.AwardBonus(employees[0], 300); // Наприклад, нарахувати 300 бонусу першому працівнику

        // Виведення інформації про премійованих працівників
        Console.WriteLine("\nТоп після нарахування бонусів:");
            foreach (var performer in topPerformersBySalary)
            {
                Console.WriteLine($"Ім'я: {performer.Name}, Посада: {performer.Position}, Бонуси: {performer.Bonus}, Зарплата: {facade.CalculateSalary(performer)}");
            }
        }
    }