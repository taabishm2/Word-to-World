from flask import Flask, request, jsonify
import os
import io
from PIL import Image

app = Flask(__name__)

def _get_sample_scene():
    return {
        "assets": [
            {
                "bundle_url": "http://34.133.232.227:8000/shapes",
                "name": "sphere",
                "scale": (1, 1, 1),
                "position": {"x": 1, "y": 3, "z": 2},
                "rotation": (0, 0, 0),
            }
        ]
    }

@app.route('/create_scene', methods=['PUT'])
def generate_initial_scene():
    data = request.get_json()
    prompt = data['prompt']
    
    print(f"user request {data}")
    
    # TODO: Call the LLM here with the provided prompt
    # (create another python file and call from here)
    # The LLM call should inclue the system prompt and the prompt provided in this request
    # In response, return a JSON similar to the one returned by _get_sample_scene() 
    
    return jsonify(_get_sample_scene()), 200

@app.route('/')
def hello_world(): 
    return "<html>Hello world</html>"

# Define the route for the POST request to upload an image
@app.route('/receive_image', methods=['POST'])
def upload_file():
    # Get the raw image data from the request body
    image_data = request.data

    # If the request does not contain any data
    if not image_data:
        return jsonify({'error': 'No image data received'})

    # Convert the raw image data to a PIL Image object
    try:
        image = Image.open(io.BytesIO(image_data))
    except Exception as e:
        print("Failed to parse image data")
        return jsonify({'error': 'Failed to parse image data'})
    
    if image.mode == 'RGBA':
        image = image.convert('RGB')

    # Save the image to a folder named "uploads" in the current working directory
    upload_folder = 'uploads/'
    if not os.path.exists(upload_folder):
        os.makedirs(upload_folder)

    # Generate a unique filename for the uploaded image
    filename = 'uploaded_image.jpg'  # You can modify the filename generation logic as per your requirements

    # Save the image
    file_path = os.path.join(upload_folder, filename)
    try:
        image.save(file_path)
    except Exception as e:
        print(f"Failed to save image data {file_path}: {e}")
        return jsonify({'error': 'Failed to save the image'})
    
    print("Everything is fine")

    return jsonify({'message': 'Image uploaded successfully', 'file_path': file_path})

if __name__ == '__main__':
    app.run(host='0.0.0.0')
