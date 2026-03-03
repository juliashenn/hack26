import cv2
from pathlib import Path
from ultralytics import YOLO

ROOT = Path(__file__).parent
model = YOLO(ROOT / 'runs' / 'excavator_v1' / 'weights' / 'best.onnx')

cap = cv2.VideoCapture(0)

while cap.isOpened():
    success, frame = cap.read()
    if not success:
        print("Failed to read from webcam.")
        break

    results = model.predict(source=frame, conf=0.25, verbose=False)
    annotated = results[0].plot()

    cv2.imshow("YOLO Webcam", annotated)

    if cv2.waitKey(1) & 0xFF == ord("q"):
        break

cap.release()
cv2.destroyAllWindows()