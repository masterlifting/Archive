# Bi-temporal database

## Introduction

Many tables in our database are bi-temporal tables. To implement this, we add extra fields to these tables: `valid_time`, `decision_time`, and `db_tx_time`.

The `valid_time` is the time period during which a row is considered valid or accurate.
The `decision_time`, on the other hand, represents the point in time when the `valid_time` was established.
Lastly, the `db_tx_time` is the span of time during which data is stored in the database.

Instead of deleting rows, our database retains all data, even when modifications are made to existing content.
Thus, updates, even those that function as record deletions, are appended to the existing table.

The following table shows the fields and index that are added to a bi-temporal table.


```postgresql
create sequence row_id_seq; 
```


```postgresql
create table bi_temporal_base_table
(
    id            bigint not null, -- Business Key
    row_id        bigint    default nextval('row_id_seq'),

    valid_time    tstzrange default ('[' || (now() at time zone 'utc') || ', infinity)')::tstzrange,
    decision_time tstzrange default ('[' || (now() at time zone 'utc') || ', infinity)')::tstzrange,
    db_tx_time    tstzrange default ('[' || (now() at time zone 'utc') || ', infinity)')::tstzrange,

    primary key (id, row_id)
);
```

In our database schema, we also introduce two additional fields: `id` and `row_id`.
The `id` acts as a business key, enabling us to uniquely identify each entity.

In our database schema, the `id` field, acting as the business key, does not provide unique identification at the table level. Instead, it is used to identify the individual entity each row represents.

However, when paired with the `row_id` field, a sequence number, the combination provides a unique identifier for each row in a table.
This pairing allows us to precisely track and manage every individual entry, even in the context of updates or modifications.

The `row_id` column is incremental and _**database wide**_ unique.

In our database setup, if you observe two rows with the same business key (`id`), the row possessing the larger `row_id` is the more recent entry as compared to the row with a smaller `row_id`. This way, the `row_id` field effectively helps us track the chronological order of data insertion or modification.

Indeed, for a given business key (`id`), if you order the data in descending sequence by the `row_id`, you will always obtain the most recent version at the top. This approach allows easy access to the latest updates or modifications for any given entity in the table.


## time axis

All time related columns are `tstzrange` type.
All time data entered into the database is set to UTC.
The database server is set to UTC.
All time related range intervals are closed on the lower bound and open on the upper bound `[lower bound, upper bound)`.
Inﬁnite bounds are always exclusive (open), even if you specify them as inclusive.

The `db_tx_time` column is the time period for which the data is stored in the database.

```
    /// business key  ....+....1....+....2....+....3....+....4....+....5∞
    /// ============  ===================================================
    /// 152 (1)       [xxxx)
    /// 152 (2)            [xxxxxxx)
    /// 152 (3)                    [xxx)
    /// 152 (4)                        [xxxxxxxxxxxxxxxxxxxxxxxxxxxx)
    /// 152 (5)                                                     [xxx)

```

When querying the database, the `db_tx_time` is used to filter the data.
When no `db_tx_time` is specified, then the current time is used for the lower bound (closed interval) and `infinity` for the upper bound.

The `valid_time` column is the time period for which the data is valid.
If the `valid_time` is `['2021-01-01 00:00:00', 'infinity')`, then the data is valid from `2021-01-01 00:00:00` until `infinity`.
When the upper bound is limited to a date / time, than after this date / time the data is no longer valid.
Please, be aware, the row is NOT deleted.

Example:


The `like` operator is used to "inject" the bi-temporal fields into a table.

```postgresql
create table address
(
    like bi_temporal_base_table including all,
    street varchar(255) not null
);
```

```postgresql
insert into address (id, street) values (1, 'street 1');
```

After the insert, the following data is stored in the database:

| id | row_id | valid_time                       | decision_time                    | db_tx_time                       | street   |
|:---|:-------|:---------------------------------|:---------------------------------|:---------------------------------|:---------|
| 1  | 1      | ["2023-08-22 13:39:00",infinity) | ["2023-08-22 13:39:00",infinity) | ["2023-08-22 13:39:00",infinity) | street 1 |

If we now update the street from `street 1` to `street 2`, the following data is stored in the database:

| id | row_id | valid_time                       | decision_time                    | db_tx_time                                     | street   |
|:---|:-------|:---------------------------------|:---------------------------------|:-----------------------------------------------|:---------|
| 1  | 1      | ["2023-08-22 13:39:00",infinity) | ["2023-08-22 13:39:00",infinity) | ["2023-08-22 13:39:00", "2023-08-22 13:40:00") | street 1 |
| 1  | 2      | ["2023-08-22 13:40:00",infinity) | ["2023-08-22 13:40:00",infinity) | ["2023-08-22 13:40:00",infinity)               | street 2 |

If we now update the street from `street 2` to `street 3`, but this change is only valid until the end of the month, the following data is stored in the database:

| id | row_id | valid_time                                    | decision_time                    | db_tx_time                                     | street   |
|:---|:-------|:----------------------------------------------|:---------------------------------|:-----------------------------------------------|:---------|
| 1  | 1      | ["2023-08-22 13:39:00",infinity)              | ["2023-08-22 13:39:00",infinity) | ["2023-08-22 13:39:00", "2023-08-22 13:40:00") | street 1 |
| 1  | 2      | ["2023-08-22 13:40:00",infinity)              | ["2023-08-22 13:40:00",infinity) | ["2023-08-22 13:40:00", "2023-08-22 13:41:00") | street 2 |
| 1  | 3      | ["2023-08-22 13:41:00","2023-09-01 00:00:00") | ["2023-08-22 13:41:00",infinity) | ["2023-08-22 13:40:00",infinity)               | street 3 |

If we now query the database on `2023-08-31`, we get a row (not deleted) entity 1 with valid content and we made this decision on `2023-08-22 13:41:00`.
If we now query the database on `2023-09-10`, we get a row (not deleted) entity 1 but the content is not valid and we made this decision on `2023-08-22 13:41:00`.

Now we delete entity 1, the following data is stored in the database:

| id | row_id | valid_time                                    | decision_time                    | db_tx_time                                     | street   |
|:---|:-------|:----------------------------------------------|:---------------------------------|:-----------------------------------------------|:---------|
| 1  | 1      | ["2023-08-22 13:39:00",infinity)              | ["2023-08-22 13:39:00",infinity) | ["2023-08-22 13:39:00", "2023-08-22 13:40:00") | street 1 |
| 1  | 2      | ["2023-08-22 13:40:00",infinity)              | ["2023-08-22 13:40:00",infinity) | ["2023-08-22 13:40:00", "2023-08-22 13:41:00") | street 2 |
| 1  | 3      | ["2023-08-22 13:41:00","2023-09-01 00:00:00") | ["2023-08-22 13:41:00",infinity) | ["2023-08-22 13:41:00", "2023-08-22 13:42:00") | street 3 |

As one can see, the db_tx_time is updated.
When we now query the database on `2023-09-10`, we get no row for entity 1.

I will leave the `decision_time` for what it is, as it is used in future versions of the database.

## Temporality in a single table

Selecting an entity by its business key (`id`) ordered desc by `row_id` limit by 1, returns the latest version of the entity.

## Temporal relationships

We can have the following temporal relations ships:

| Source table | Target table | description |
|--------------|--------------|-------------|
| bi-temporal  | bi-temporal  |             |
| standard     | bi-temporal  |             |
| bi-temporal  | standard     |             |
| standard     | standard     |             |

In addition to 

We can have a n..m relationship between the source and target table.
All other relationships, 0,1..n or 1..n or n..m are derived from a n..m relationship with a constraint.
To implement a n..m relationship, we cannot fall back to a standard database construct.
We need a special implementation.

I now elaborate on the bi-temporal to bi-temporal n..m relationship.

An entity is identified by its business key (`id`).

A relation ship itself is a bi-temporal entity.







