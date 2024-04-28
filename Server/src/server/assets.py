from collections import defaultdict

import csv, json

ASSETS_CSV_PATH = "src/assets/asset_metadata.csv"
ASSETS_JSON_PATH = "src/assets/assets.json"


def get_dimensions(row):
    scale = float(row[5])
    a, b, c = float(row[2]), float(row[3]), float(row[4])
    a, b, c = a * scale, b * scale, c * scale
    return {"x": round(a, 3), "y": round(b, 3), "z": round(c, 3)}


def build_asset_json():
    print("\nBuilding assets.json...")
    with open(ASSETS_CSV_PATH, "r") as file:
        reader = csv.reader(file)
        next(reader)  # Skip the header row
        rows = list(reader)

        full_dict = {"assets": []}
        for row in rows:
            full_dict["assets"].append(
                {
                    "name": row[0],
                    "description": row[6],
                    "dimensions": get_dimensions(row),
                }
            )

        with open(ASSETS_JSON_PATH, "w") as json_file:
            json.dump(full_dict, json_file)

        print("assets.json built successfully")


def get_scales():
    scale_dict = dict()
    
    with open(ASSETS_CSV_PATH, "r") as file:
        reader = csv.reader(file)
        next(reader)  # Skip the header row
        
        for row in list(reader):
            scale_dict[row[0]] = row[5]
    
    return scale_dict


build_asset_json()
