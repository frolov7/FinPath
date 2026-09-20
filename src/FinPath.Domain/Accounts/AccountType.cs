namespace FinPath.Domain.Accounts
{
    // <summary>
    // Поддерживаемые типы финансовых счетов
    // </summary>
    public enum AccountType
    {
        // <summary>
        // Наличные денежные средства пользователя
        // </summary>
        Cash = 1,
        // <summary>
        // Дебетовая карта с собственными денежными средствами пользователя
        // </summary>
        DebitCard = 2,
        // <summary>
        // Обычный банковский счет
        // </summary>
        BankAccount = 3,
        // <summary>
        // Накопительный счет для хранения и накопления денежных средств
        // </summary>
        SavingAccount = 4,
        // <summary>
        // Кредитная карта с заемными денежными средствами
        // </summary>
        CreditCard = 5,
        // <summary>
        // Счет для хранения инвестиционных активов
        // </summary>
        InvestmentAccount = 6
    }
}
