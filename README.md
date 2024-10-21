- Необходимо опубликовать проект **MdPWASM** в директорию _MdPWASM/bin/Release/net8.0/publish_ с помощью команды `dotnet publish`
- На начальном этапе для получения модуля **Blazor WebAssembly** клиентской частью на **React** необходимо поедлиться этим файлами. Можно запустить локальный сервер на localhost:8080 с включенным CORS. 

# Настройка http сервера 
1) Создание файла:
`touch server.js
`
2) Добавление кода в **server.js**
```const cors = require('cors');
const express = require('express');
const app = express();

// Опции CORS для разрешения запросов с http://localhost:5173
const corsOptions = {
    origin: 'http://localhost:5173', // Укажи конкретный источник (React)
    credentials: true, // Разрешаем отправку учетных данных
};

// Включаем CORS с заданными опциями
app.use(cors(corsOptions));

// Указываем папку, откуда будем отдавать статические файлы
app.use(express.static('.')); // Текущая директория

// Запускаем сервер на порту 8080
app.listen(8080, () => {
    console.log('Сервер запущен на http://localhost:8080');
});
```
3) Установка зависимостей: `npm install express cors
`
4) Запуск сервера: `node server.js
`

_Примечание: запускать сервер стоит в публикуемой папке wwroot_
