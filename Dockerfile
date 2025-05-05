FROM nginx:alpine

# Remove the default configuration file that comes with nginx
RUN rm /etc/nginx/conf.d/default.conf

# Copy your custom nginx configuration into the container.
# This file will override the default nginx settings.
COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY ./Builds/Release/Web /usr/share/nginx/html/

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]