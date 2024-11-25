using System;
using System.IO;  // Для работы с файловой системой
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;

class Program
{
    // Путь к файлу для логирования
    static string logFilePath = "bot_logs.txt";

    static async Task Main(string[] args)
    {
        // Ваш токен от BotFather
        string token = "API TUT !!!";

        // Инициализация клиента Telegram Bot API
        var botClient = new TelegramBotClient(token);

        // Логирование времени запуска бота
        LogToFile($"Bot started at {DateTime.Now}");
        Console.WriteLine($"Bot started at {DateTime.Now}");

        // Подписка на получение обновлений с использованием делегатов
        botClient.StartReceiving(
            HandleUpdatesAsync,
            HandleErrorAsync
        );

        Console.ReadLine(); // Бот будет работать, пока не закроете консоль
    }

    // Обработка обновлений
    static async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message != null)
        {
            var message = update.Message;

            // Логируем информацию о сообщении в консоль и в файл
            LogToFile($"Message received from {message.From.Username}: {message.Text}");
            Console.WriteLine($"Message received from {message.From.Username}: {message.Text}");

            if (message.Text == "/start")
            {
                // Отправка описания бота
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Привет! Я — бот, который может делать следующее:\n" +
                          "- Отправлять сообщения с кнопками\n" +
                          "- Обрабатывать нажатие кнопок\n\n" +
                          "Нажми на одну из кнопок ниже, чтобы начать!",
                    replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                        new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton.WithCallbackData("Кнопка 1", "button_1"),
                            InlineKeyboardButton.WithCallbackData("Кнопка 2", "button_2")
                        }
                    })
                );
            }
            else
            {
                // Логика для обработки других сообщений
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new InlineKeyboardButton[] // Ряд с кнопками
                    {
                        InlineKeyboardButton.WithCallbackData("Кнопка 1", "button_1"),
                        InlineKeyboardButton.WithCallbackData("Кнопка 2", "button_2")
                    }
                });

                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Привет! Нажми на одну из кнопок:",
                    replyMarkup: inlineKeyboard
                );
            }
        }
        else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
        {
            var callbackQuery = update.CallbackQuery;

            // Логируем информацию о callback в консоль и в файл
            LogToFile($"Callback received from {callbackQuery.From.Username}: {callbackQuery.Data}");
            Console.WriteLine($"Callback received from {callbackQuery.From.Username}: {callbackQuery.Data}");

            if (callbackQuery.Data == "button_1")
            {
                await botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: "Вы нажали Кнопку 1"
                );
            }
            else if (callbackQuery.Data == "button_2")
            {
                await botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: "Вы нажали Кнопку 2"
                );
            }
        }
    }

    // Обработка ошибок
    static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        // Логируем ошибку в консоль и в файл
        Console.WriteLine($"Произошла ошибка: {exception.Message}");
        LogToFile($"Error: {exception.Message}");
        return Task.CompletedTask;
    }

    // Логирование в файл и вывод в консоль
    static void LogToFile(string message)
    {
        try
        {
            // Запись сообщения в файл с добавлением времени
            System.IO.File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Не удалось записать в лог: {ex.Message}");
        }
    }
}
