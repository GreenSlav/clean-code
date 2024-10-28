# Frontend/Dockerfile

# Этап сборки
FROM node:18 AS build
WORKDIR /app

# Копирование package.json и package-lock.json
COPY ./wasm_proj/package*.json ./
RUN npm install

# Передача переменной на этапе сборки
ARG VITE_REACT_APP_BACKEND_URL
ENV VITE_REACT_APP_BACKEND_URL=${VITE_REACT_APP_BACKEND_URL}

# Копирование исходного кода
COPY ./wasm_proj ./

# Сборка приложения
RUN npm run build

# Этап продакшн
FROM nginx:alpine AS production

# Копирование собранного приложения из папки dist в папку Nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Открытие порта
EXPOSE 80

# Запуск Nginx
CMD ["nginx", "-g", "daemon off;"]
