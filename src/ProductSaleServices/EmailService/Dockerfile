FROM openjdk:16-alpine3.13

WORKDIR /app

EXPOSE 9001

COPY ["EmailService/.mvn/", ".mvn"]
COPY ["EmailService/mvnw", "./"]
COPY ["EmailService/pom.xml", "./"]
RUN ./mvnw dependency:go-offline

COPY ["EmailService/src", "./src"]

CMD ["./mvnw", "spring-boot:run"]