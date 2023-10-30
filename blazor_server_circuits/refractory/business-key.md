---

## Developer's Documentation: 64-bit Business Key and UUID Mapping

### Table of Contents

1. Introduction
2. The Intention Behind Using 64-bit Business Keys
3. The Need for UUID Mapping
4. Security Vulnerabilities of Sequential IDs
5. Implementation Details
6. Conclusion

---

### 1. Introduction

In our application architecture, it's imperative to maintain a balance between performance and security. To achieve this
balance, we've chosen to utilize a combination of 64-bit internal business keys for identifying entities and UUIDs for
external access. This documentation will walk through the intention behind these choices and highlight the security
vulnerabilities of alternative methods.

### 2. The Intention Behind Using 64-bit Business Keys

**Performance and Scalability**: One of the primary reasons for using a 64-bit business key is the sheer number of
unique IDs it offers without occupying much space. This vast space allows for scalability in the future without the fear
of running out of unique identifiers.

**Internal Efficiency**: For intra-application and database references, using 64-bit business keys are faster and more
efficient compared to string-based UUIDs, mainly because of smaller index size and faster lookups.

### 3. The Need for UUID Mapping

**Security Through Obscurity**: UUIDs make it computationally infeasible for someone to guess the next or previous ID,
thereby adding an extra layer of security.

**External Access Control**: By using UUIDs for all external access, we can ensure that unauthorized users cannot guess
the sequential IDs, which would otherwise give them access to data they shouldn't see.

**Clear Boundary Between Internal and External Access**: Having a separate identifier for external requests allows the
system to clearly differentiate between requests that come from outside the Kubernetes cluster and those that are
internal, ensuring the appropriate level of access control and validation is applied.

### 4. Security Vulnerabilities of Sequential IDs

Entities with predictable IDs can be a major security risk. A hacker can easily guess the ID sequence and access
unauthorized data. Sequential IDs can also leak information about the business, like the number of records or the rate
of new record creation.

### 5. Implementation Details

- **Internal Business Key Generation**: Every new entity gets a new 64-bit business key, which is sequentially
  generated.

- **UUID Mapping Process**: As soon as an entity is generated with a business key, a corresponding UUID is generated and
  mapped. This UUID becomes the reference point for any external access.

- **Database Storage**: Within the database, references to other entities are stored using the 64-bit business key. A
  separate table or associated mapping holds the relationship between the business key and the UUID.

- **Routing**: When a request comes from an external source, it will always use the UUID. The system will map the UUID
  back to the business key for any internal operations or database lookups.

### 6. Conclusion

Using a combination of 64-bit business keys and UUIDs allows us to harness the performance benefits of sequential IDs
while safeguarding against unauthorized access risks posed by predictable IDs. Always ensure that any external access
uses UUIDs, and be vigilant about maintaining the mapping integrity between UUIDs and business keys.

---

**Note**: Ensure to review the latest security best practices regularly and adjust the system's architecture as
necessary to mitigate new or evolving risks.
