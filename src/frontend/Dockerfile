# Этап сборки фронтенда
FROM node:20 AS build
WORKDIR /app

# Копируем package.json и package-lock.json
COPY package.json package-lock.json ./

# Удаляем старый package-lock.json (если он содержит macOS-специфичные зависимости)
RUN rm -f package-lock.json

# Устанавливаем зависимости строго для Linux
RUN npm install --force --legacy-peer-deps

# Передаем переменную окружения на этапе сборки
ARG VITE_REACT_APP_BACKEND_URL
ENV VITE_REACT_APP_BACKEND_URL=${VITE_REACT_APP_BACKEND_URL}

# Копируем исходный код
COPY . .

# Сборка фронтенда
RUN npm run build

FROM nginx:alpine AS production

# Копируем конфиг Nginx
# 📌 Копируем конфиг Nginx в правильную папку `/etc/nginx/conf.d/`
COPY nginx.conf /etc/nginx/conf.d/default.conf

# 📌 Копируем собранное приложение в Nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Открываем порт
EXPOSE 80

# Запускаем Nginx
CMD ["nginx", "-g", "daemon off;"]