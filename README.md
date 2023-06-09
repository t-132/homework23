## Общая структура проекта для домашнего задания по параллельной загрузке данных

### Постановка задачи

Цель: Сделать параллельный обработчик файла с данными клиентов на основе подготовленного проекта с архитектурой. 
Задание поможет отработать основные инструменты параллелизма на реалистичной задаче.
Каждая строка файла содержит: 
- id (целое число) 
- ФИО (строка), 
- Email (строка) 
- Телефон (строка). 

Данные отсортированы по id. Нужно десериализовать данные клиента в объект и передать объект в метод класса, который сохраняет его в БД.

### Задание
1. Запуск генератора файла через создание процесса, сделать возможность выбора в коде, как запускать генератор, процессом или через вызов метода.
2. Распараллеливаем обработку файла по набору диапазонов Id, то есть нужно, чтобы файл разбивался на диапазоны по Id и обрабатывался параллельно через Thread, обработка предполагает сохранение в БД через вызов репозитория.  Хорошо сделать настройку с количеством потоков, чтобы можно было настроить оптимальное количество потоков под размер файла с данными. Предусмотреть обработку ошибок и механизм попыток для повторного сохранения в случае ошибки. Проверить обработку на файле, в котором 1 млн. записей, при сдаче задания написать время, за которое был обработан файл и количество потоков.
3. Добавить сохранение в реальную БД, в качестве БД выбрать SQL Lite для простоты тестирования и реализации.
4. По желанию вместо SQL Lite проверить работу приложения на полноценной БД, например, MS SQL Server или PostgreSQL, увеличив размер файла до 3 млн записей. При сдаче работы написать о результатах, возникали ли длительные блокировки при записи в таблицу и за какое время происходила загрузка.(*) 
5. Дать обратную связь по 1-му домашнему заданию других студентов на курсе.

### Инструкция
1. Сделать форк этого репозитория.
2. Реализовать 1 пункт задания, сделав в main проекта запуск процесса-генератора файла, его нужно будет собрать отдельно и передать в программу путь к .exe файлу, также сделать в `Main` вызов кода генератора из подключенного проекта, выбор между процессом или вызовом метода сделать настройкой (например аргумент командной строки или файл с настройками) со значением по умолчанию для метода.
3. Реализовать 2 пункт задания, сделав свои реализации для `IDataLoader` и `IDataParser`. Лучше всего десериализовать данные из файла в коллекцию и передать в конструктор реализации `IDataLoader`, а затем уже в реализации разбить коллекцию на набор подколлекций согласно количеству потоков, чтобы каждую подколлекцию обрабатывал свой поток. Предусмотреть обработку ошибок в обработчике потока и перезапуск по ошибке с указанием числа попыток. При обработке поток должен вызывать сохранение данных через репозитории.
4. Реализовать 3 пункт задания, сделав дополнительную реализацию для `ICustomerRepository` и инициализацию БД при старте приложения, можно использовать EF, в этом случае DbContext должен создаваться на поток, чтобы не было проблем с конкуренцией, так как DbContext не потокобезопасен. При многопоточной записи в SQLite могут быть проблемы с блокировкой файла базы, чтобы этого избежать нужно не забывать про using при создании connection к БД, если это делается через EF, то DbContext должен создаваться в using. При активной записи все равно могут быть блокировки, для этого мы реализуем механизм попыток, когда блокировка будет снята, даже если было исключение, то при следующей попытке поток запишет данные, это актуально и для больших баз по нагрузкой. 
5. По желанию реализовать 4 пункт задания.
5. По желанию дать обратную связь по 1-му домашнему заданию других студентов на курсе, можно найти репозитории по форкам к этому репозиторию. Обратную связь можно описать, создав issue к репозиторию, например, 
https://gitlab.com/devgrav/Otus.Teaching.Concurrency.Queue/issues/1. Чтобы обратная связь была качественной обязательно нужно похвалить работу, написав, что сделано хорошо и написать, что можно улучшить с пояснениями почему это сделает работу более качественной. Эти рекомендации работают и для code review, так как позволяют более конструктивно обсуждать коммиты.

Некоторые вещи можно посмотреть в проекте, который был рассмотрен на занятии:
https://gitlab.com/devgrav/Otus.Teaching.Concurrency.Queue

### Оценка 
1. 2 балла
2. 3 балла
3. 2 балла
4. 2 балла
5. 1 балл

Всего 10 баллов, для приема задания достаточно 7 баллов.

### Otus.Teaching.Concurrency.Import.Loader

Консольное приложение, которое должно сгенирировать файл с данными и запустить загрузку из него через реализацию `IDataLoader`.

### Otus.Teaching.Concurrency.Import.DataGenerator

Библиотека, в которой должна определена логика генерации файла с данными, в базовом варианте это XML.

### Otus.Teaching.Concurrency.Import.DataGenerator.App

Консольное приложение, которое позволяет при запуске отдельно выполнить генерацию файла из `DataGenerator` библиотеки.

### Otus.Teaching.Concurrency.Import.DataAccess

Библиотека, в которой находится доступ к базе данных и файлу с данными.

### Otus.Teaching.Concurrency.Import.Core

Библиотека, в которой определены сущности БД и основные интерфейсы, которые реализуют другие компоненты.
