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
                    "scale": 1.3,
                    "location": (1, 3, 2),
                    "rotation": (0, 0, 0),
                    "texture": {},
                }
            ]
        }
    }
    
def _refine_scene(request, is_auto=False):
    image_x_pos = request.files['x_pos']
    image_x_neg = request.files['x_neg']
    image_y_pos = request.files['y_pos']
    image_y_neg = request.files['y_neg']
    image_z_pos = request.files['z_pos']
    image_z_neg = request.files['z_neg']
    
    if is_auto: prompt = "Auto refine prompt"
    else: prompt = request.json['prompt']
    
    # TODO: Call the LLM here
    
    return 

@app.route('/test-server', methods=['GET'])
def home():
    return jsonify({'message': 'Server is running!'}), 200

@app.route('/create-scene', methods=['POST'])
def generate_initial_scene():
    data = request.get_json()
    prompt = data['prompt']
    
    # TODO: Call the LLM here
    
    return jsonify(_get_sample_scene()), 200

# Automatically refine the scene based on images
@app.route('/auto-refine-scene', methods=['POST'])
def auto_refine_scene():    
    response = _refine_scene(request, is_auto=True)
    return jsonify(response), 200

# Refine scene based on user input + images
@app.route('/refine-scene', methods=['POST'])
def refine_scene():
    response = _refine_scene(request, is_auto=False)
    return jsonify(response), 200

if __name__ == '__main__':
    app.run(debug=True)
