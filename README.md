- Необходимо опубликовать проект **MdPWASM** в директорию _clean-code/src/Markdown/MdProcessorWebApi/wwwroot_ с помощью команды `dotnet publish`
- На начальном этапе для получения модуля **Blazor WebAssembly** клиентской частью на **React** необходимо поделиться этим файлами. Можно запустить локальный сервер на localhost с включенным CORS.
- Запуск клиента на **React**:
1) cd clean-code/src/Frontend
2) npm install
3) npm run dev

__UPD: Проект MdProcessorWebApi уже способен раздавать необходимые для запуска wasm файлы в браузере клиенту на React по пути `http://localhost:{общий_порт}/api/Blazor/resource/{нужный_файл}`. Необходимо лишь согласовать порты (пока, в дальнейшем будет перманентный домен) на которых сервер будет работать и клиент ожидать файлы__ 
