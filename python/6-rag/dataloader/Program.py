from __future__ import annotations

import json
import random
from dataclasses import dataclass
from pathlib import Path

from azure.data.tables import TableServiceClient

CONNECTION_STRING = "<connection_string>"  # Replace with your Azure Table Storage connection string


@dataclass
class Figurine:
    Id: str
    Name: str
    Description: str
    Price: float


def load_figurines() -> list[Figurine]:
    payload = Path("figurines.json").read_text(encoding="utf-8")
    return [Figurine(**item) for item in json.loads(payload)]


def main() -> None:
    figurines = load_figurines()
    service_client = TableServiceClient.from_connection_string(CONNECTION_STRING)

    figurines_table = service_client.get_table_client("Figurines")
    orders_table = service_client.get_table_client("Orders")
    order_figurines_table = service_client.get_table_client("OrderFigurines")

    for figurine in figurines:
        figurines_table.upsert_entity(
            {
                "PartitionKey": "Figurines",
                "RowKey": figurine.Id,
                "Name": figurine.Name,
                "Description": figurine.Description,
                "Price": figurine.Price,
            }
        )

    customers = [
        ("C001", "Luke Johnson"),
        ("C002", "Leia Parker"),
        ("C003", "Han Richards"),
        ("C004", "Ben Smith"),
        ("C005", "Yoda Masterson"),
        ("C006", "Rey Fisher"),
        ("C007", "Anakin Skywalker"),
        ("C008", "Padmé Amidala"),
        ("C009", "Lando Calrissian"),
        ("C010", "Obi Wan"),
    ]

    rng = random.Random()

    for i in range(10):
        order_id = 60 + i
        customer_id, customer_name = customers[i]

        if order_id == 66:
            chosen = [f for f in figurines if f.Id == "F019"]
        else:
            sample_count = rng.randint(1, 4)
            chosen = rng.sample(figurines, k=sample_count)

        total_cost = sum(item.Price for item in chosen)
        orders_table.upsert_entity(
            {
                "PartitionKey": "Orders",
                "RowKey": str(order_id),
                "CustomerID": customer_id,
                "CustomerName": customer_name,
                "TotalCost": total_cost,
            }
        )

        for figurine in chosen:
            order_figurines_table.upsert_entity(
                {
                    "PartitionKey": str(order_id),
                    "RowKey": figurine.Id,
                    "FigurineName": figurine.Name,
                    "Price": figurine.Price,
                }
            )

    print("Data insertion complete.")


if __name__ == "__main__":
    main()

