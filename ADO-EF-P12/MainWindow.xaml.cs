using ADO_EF_P12.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ADO_EF_P12
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataContext dataContext;
        public ObservableCollection<Pair> Pairs { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            dataContext = new();
            Pairs = new();
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DepartmentsCountLabel.Content = dataContext.Departments.Count().ToString();
            TopChiefCountLabel.Content =
                dataContext
                .Managers
                .Where(   // predicate - функція, що повертає bool
                    manager => manager.IdChief == null
                )  // ?? для кожного елементу здійснюється порівняння??
                   // Ні! з аналізу предикату будується SQL запит
                .Count()
                .ToString();


        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            /* Вивести Прізвище - І.П.(ініціали) співробітників бухгалтерії
             */
            var query = dataContext
                .Managers
                .Where(m => m.IdMainDep == Guid.Parse("131ef84b-f06e-494b-848f-bb4bc0604266"))
                .Select(                        // Select - правило перетворення
                    m =>                        // на "вході" елемент попередньої
                    new Pair                    // колекції (m - manager), а на
                    {                           // виході - результат лямбди
                        Key = m.Surname,        // 
                        Value = $"{m.Name[0]}. {m.Secname[0]}."
                    }
                );
            // query - "правило" побудови запиту. Сам запит ані надісланий, ані
            // одержано результати

            Pairs.Clear();
            foreach ( var pair in query )  // цикл-ітератор запускає виконання запиту
            {                              // саме з цього моменту іде звернення до БД
                Pairs.Add(pair);
            }
            /* Особливості:
             * - LINQ запит можна зберігти у змінній, сам запит є "правилом" і не
             *    ініціює звернення до БД
             * - LINQ-to-Entity вживає приєднаний режим, тобто кожен запит надсилається
             *    до БД, а не до "скачаної" колекції
             * - Виконання запиту здійснюється шляхами:
             *   = виклик агрегатора (.Count(), .Max(), тощо)
             *   = виклик явного перетворення (.ToList(), ToArray(), тощо)
             *   = запуск циклу з ітерування запиту
             *   
             * -- Фільтрування (.Where) краще здійснювати за індексованими полями,
             *      у т.ч. за первинним ключем (який автоматично індексується)
             * -- Інструкція .Select є перетворювачем, а не запуском запиту     
             */
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            /* Вивести Прізвище І.П. -- Назва робочого відділу
             * Обмежити вибірку першими 10 рядками та пропустити 
             * перших трьох
             */
            var query = dataContext           // Запит з поєднанням таблиць
                .Managers                     // Ліва таблиця
                .Join(                        // операція поєднання
                    dataContext.Departments,  //  права таблиця
                    m => m.IdMainDep,         //  outerKey - зовн. ключ з лівої таблиці
                    d => d.Id,                //  innerKey - внутр. ключ правої таблиці
                    (m, d) =>                 // selector - правило перетворення
                       new Pair               //  пари сутностей, для яких зареєстровано
                       {                      //  з'єднання (join)
                           Key = m.Surname,   // 
                           Value = d.Name     // 
                       }
                )
                .Skip(3)                      // пропустити 3 елементи
                .Take(10);                    // обмеження - 10 елементів з вибірки

            Pairs.Clear();
            foreach( var pair in query )
            {
                Pairs.Add(pair);
            }
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            /* Вивести відомості 
             * Прізвище І.П. співробітника -- Прізвище І.П. керівника
             * Впорядкувати за абеткою по співробітниках
             */
            var query = dataContext        // SELECT ... FROM
                .Managers                  // Managers as m
                .Join(                     // JOIN
                    dataContext.Managers,  // Managers as chief ON
                    m => m.IdChief,        // m.IdChief =
                    chief => chief.Id,     //             chief.Id
                    (m, chief) => new Pair
                    {
                        Key = m.Surname,
                        Value = chief.Surname
                    }
                )  // .OrderBy(pair => pair.Key)  // у цьому місці працює лише якщо
                   // у  pair.Key - посилання на поле entity
                   // наприклад m.Surname
                .ToList()  // запускає запит та перетворює результат на колекцію
                .OrderBy(pair => pair.Key);  // у цьому місці - інший LINQ, який діє на
                                             // колекцію, а не на запит SQL

            Pairs.Clear();
            foreach( var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            /* Вивести дані Дата реєстрації -- Прізвище І.Б. співробітника
             * Впорядкувати по даті - останні зареєстровані ідуть першими
             * Обмежити вибірку 7ма рядками
             * (7 останніх зареєстрованих)
             */
            /* Д.З. Використовуючи LINQ-to-Entity та створену БД
             * Вивести дані  
             * Прізвище І.Б. співробітника  --  Назва відділу (за сумісництвом: SecDep)
             * Впорядкувати по назві відділу
             */
        }
    }

    public class Pair
    {
        public String Key { get; set; } = null!;
        public String? Value { get; set; }
    }
}
/* Д.З. По БД, створеної на зустрічі, вивести (у "моніторі БД") засобами LINQ
 * Кількість підлеглих (осіб, що мають керівника)
 * Кількість співробітників ІТ-відділу (як основних, так і сумісників)
 * Кількість співробітників, які працюють у двох відділах (основний та сумісний)
 */
