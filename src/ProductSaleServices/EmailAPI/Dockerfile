FROM openjdk:16-alpine3.13

WORKDIR /app

EXPOSE 9000

COPY ["EmailAPI/.mvn/", ".mvn"]
COPY ["EmailAPI/mvnw", "./"]
COPY ["EmailAPI/pom.xml", "./"]
RUN ./mvnw dependency:go-offline

COPY ["EmailAPI/src", "./src"]

CMD ["./mvnw", "spring-boot:run"]