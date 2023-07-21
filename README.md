Платформа: .NET6, React ^16.0.0

Проект налаштований на запуск доменів:
1. https://*:443. Наприклад https://localhost
2. http://*:80. Наприклад http://localhost

Зміна конфігурації доменної адресації можлива безпосередньо через IIS або через appsettings.json (Рекомендується прибрати розділ Kestrel при використанні IIS)

Перш за все вам потрібно налаштувати config.js
- Розділ ColorTags відповідає за визначення кольорів консольного тексту.
- Розділ MinecraftServers дозволяє встановити конфігурацію сервера для запуску програми. Ці параметри повинні збігатися з параметрами сервера.

- JavaHandler - призначення виконуючого програми java
- Arguments – параметри запуску сервера
- WorkingDirectory - папка, в якій знаходиться корінь вашого сервера
- ServerName - ім'я сервера, що відображається
- ServerIp - ip сервера, використовується для відображення та моніторингу сервера
- ServerPort - порт роботи сервера, що використовується для відображення та моніторингу сервера
- ServerCore - ім'я файлу (з розширенням) виконуючого файлу сервера
- SchemDirectory - папка керування схематиками сервера (Доступно при використанні плагіна WorldEdit)

Можливості:
- відстеження споживання ОЗУ процесом сервера
- Моніторинг сервера та отримання базової інформації
- Кешування базової інформації сервера
- Консольна переадресація input/output
- При позаплановій зупинці програми сайту, запущені процеси серверів будуть знищені
- Двостороннє з'єднання консольного потоку
- Завантаження та відображення файлів schem, schematic на сервер

Примітка:
- RCON система не реалізована
- При великому потоці повідомлень може навантажуватися ваша система (на строні клієнта)
- Повідомлення сервера не зберігаються і надсилаю в момент їх отримання

------------------------------------------------------------------------
Платформа: .NET6, React ^16.0.0

Проект настроен на запуск по доменам:
1. https://*:443. Например https://localhost
2. http://*:80. Например http://localhost

Изменение конфигурации доменной адресации возможно непосредственно через IIS, или через appsettings.json (Рекомендуется убрать раздел Kestrel при использовании IIS)

В первую очередь вам нужно настроить config.js
- Раздел ColorTags отвечает за определение цветов текста консольного вывода.
- Раздел MinecraftServers позволяет задать конфигурацию сервера для запуска приложения. Эти параметры должны совпадать с параметрами вашнго сервера.

- JavaHandler - назначение исполняющего приложения java
- Arguments - параметры запуска сервера
- WorkingDirectory - папка, в которой находиться корень вашего сервера
- ServerName - отображаемое имя сервера
- ServerIp - ip сервера, используется для отображения и мониторинга сервера
- ServerPort - порт работы сервера, используется для отображения и мониторинга сервера
- ServerCore - имя файла (с расширением) исполняющего файла сервера
- SchemDirectory - папка управления схематиков сервера (Доступно при использовании плагина WorldEdit)

Возможности:
- Отслеживание потребление ОЗУ процессом сервера
- Мониторинг сервера и получение базовой информации
- Кеширование базовой информации сервера
- Консольная переадресация input/output
- При внеплановой остановки приложения сайта, запущенные процессы серверов будут уничтожены
- Двусторонее соединение консольного потока
- Загрузка и отображение файлов schem, schematic на сервер

Примечание:
- RCON система не реализована
- При большом потоке сообщений - может нагружаться ваша система (на строне клиента)
- Сообщения сервера не сохраняются и отправляю в моменте их получения

------------------------------------------------------------------------

Platform: .NET6, React ^16.0.0

The project is configured to run across domains:
1. https://*:443. For example https://localhost
2. http://*:80. For example http://localhost

Changing the domain addressing configuration is possible directly through IIS, or through appsettings.json (Recommended to remove the Kestrel section when using IIS)

First of all you need to set up config.js
- The ColorTags section is responsible for defining the colors of the console output text.
- The MinecraftServers section allows you to set the server configuration for running the application. These settings must match your server settings.

- JavaHandler - assignment of the executing java application
- Arguments - server startup parameters
- WorkingDirectory - folder where the root of your server is located
- ServerName - server display name
- ServerIp - server ip, used to display and monitor the server
- ServerPort - server operation port, used to display and monitor the server
- ServerCore - file name (with extension) of the server executable file
- SchemDirectory - server schema management folder (Available when using the WorldEdit plugin)

Possibilities:
- Tracking the RAM consumption of the server process
- Server monitoring and getting basic information
- Caching basic server information
- Console redirect input/output
- In case of an unscheduled stop of the site application, running server processes will be destroyed
- Two-way console stream connection
- Uploading and displaying schem, schematic files to the server

Note:
- RCON system not implemented
- With a large flow of messages - your system can be loaded (on the client side)
- Server messages are not saved and sent as soon as they are received
