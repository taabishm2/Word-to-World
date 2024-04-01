from flask import Flask, jsonify, request

app = Flask(__name__)

def _get_sample_scene():
    return {
        "scene": {
            "id": 1, # Could use to store state about the scene
            "assets": [
                {
                    "bundle_url": "https://example.com/asset_bundle",
                    "name": "asset_name",
                    "scale":(1, 1, 1),
                    "location": (1, 3, 2),
                    "rotation": (0, 0, 0),
                }
            ]
        }
    }

@app.route('/create-scene', methods=['POST'])
def generate_initial_scene():
    data = request.get_json()
    prompt = data['prompt']
    
    # TODO: Call the LLM here with the provided prompt
    # (create another python file and call from here)
    # The LLM call should inclue the system prompt and the prompt provided in this request
    
    return jsonify(_get_sample_scene()), 200

if __name__ == '__main__':
    app.run(debug=True)
