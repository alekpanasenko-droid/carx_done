# Carx Lab — Forest Cabin

Минимальный проект Unity 2022.3 LTS с готовой сценой **ForestCabin**: домик, лес, трава, камни, дорожка и простое освещение.

## Быстрый запуск

1. Откройте эту папку через Unity Hub в Unity **2022.3 LTS** (Built-in Render Pipeline).
2. Откройте `Assets/Scenes/ForestCabin.unity`.
3. Нажмите Play. Скрипт создаёт окружение из примитивов и процедурных простых текстур.

## Бесплатные Asset Store ассеты

Базовая сцена намеренно работает без внешних скачиваний. После импорта можно заменить примитивы на модели из бесплатных наборов:

- [EmbersStorm – Free Nature Pack](https://assetstore.unity.com/packages/3d/environments/embersstorm-free-nature-pack-369910) — деревья, трава, камни и брёвна.
- [Low-Poly Simple Nature Pack](https://assetstore.unity.com/packages/3d/environments/landscapes/low-poly-simple-nature-pack-162153) — дополнительная растительность.
- [UK Terraced Houses Pack FREE](https://assetstore.unity.com/packages/3d/environments/urban/uk-terraced-houses-pack-free-63481) — бесплатные домики.

Ассеты распространяются по Unity Asset Store EULA и поэтому не включены в репозиторий. Импортируйте их через **Window → Package Manager → My Assets**, затем замените объекты в дочерних группах `Forest`, `Grass` или `Cabin`.

## Структура

- `Assets/Scenes/ForestCabin.unity` — стартовая сцена.
- `Assets/Scripts/ForestSceneBuilder.cs` — генератор окружения.

