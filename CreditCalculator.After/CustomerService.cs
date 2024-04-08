namespace CreditCalculator.After;

public class CustomerService
{
    public bool AddCustomer(
        string firstName,
        string lastName,
        string email,
        DateTime dateOfBirth,
        int companyId)
    {
        if (!IsNameValid(firstName, lastName))
            return false;


        if (!IsEmailValid(email))
            return false;

        if (!ValiDdateOfBirth(dateOfBirth))
            return false;

        var age = CalculatetAge(dateOfBirth);
        if (!LegalAge(age))
            return false;



        var companyRepository = new CompanyRepository();
        var company = companyRepository.GetById(companyId);

        var customer = new Customer
        {
            Company = company,
            DateOfBirth = dateOfBirth,
            EmailAddress = email,
            FirstName = firstName,
            LastName = lastName
        };

        if (company.Type == "VeryImportantClient")
        {
            // Skip credit check
            customer.HasCreditLimit = false;
        }
        else if (company.Type == "ImportantClient")
        {
            // Do credit check and double credit limit
            customer.HasCreditLimit = true;
            var creditService = new CustomerCreditServiceClient();

            var creditLimit = creditService.GetCreditLimit(
                customer.FirstName,
                customer.LastName,
                customer.DateOfBirth);

            creditLimit *= 2;
            customer.CreditLimit = creditLimit;
        }
        else
        {
            // Do credit check
            customer.HasCreditLimit = true;
            var creditService = new CustomerCreditServiceClient();

            var creditLimit = creditService.GetCreditLimit(
                customer.FirstName,
                customer.LastName,
                customer.DateOfBirth);

            customer.CreditLimit = creditLimit;
        }

        if (customer.HasCreditLimit && customer.CreditLimit < 500)
        {
            return false;
        }

        var customerRepository = new CustomerRepository();
        customerRepository.AddCustomer(customer);

        return true;
    }

    public static bool IsNameValid(string firstName, string lastName)
    {
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            return false;
        }
        return true;
    }

    public static bool IsEmailValid(string email)
    {
        if (!email.Contains('@') && !email.Contains('.'))
        {
            return false;
        }
        return true;
    }

    public static bool LegalAge(int age)
    {
        if (age < 21)
        {
            return false;
        }
        return true;

    }

    public static int CalculatetAge(DateTime dateOfBirth) 
    {
        var now = DateTime.Now;
        var age = now.Year - dateOfBirth.Year;
        if (now.Month < dateOfBirth.Month ||
            now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)
        {
            age--;
        }
        return age;

    }

    public static bool ValiDdateOfBirth(DateTime dateOfBirth) {
        var now = DateTime.Now;
        if (now < dateOfBirth)
            return false;

        return true;
    }

}