const express = require('express');
const AWS = require('aws-sdk');
const multer = require('multer');
const multerS3 = require('multer-s3');
const app = express();
const port = 3000;

AWS.config.update({
    accessKeyId: 'YOUR_ACCESS_KEY',
    secretAccessKey: 'YOUR_SECRET_KEY',
    region: 'YOUR_REGION'
});

const s3 = new AWS.S3();

const upload = multer({
    storage: multerS3({
        s3: s3,
        bucket: 'bucket_rebeil',
        key: function (req, file, cb) {
            cb(null, file.originalname);
        }
    })
});

app.post('/upload', upload.array('file',1), function (req, res, next) {
    res.send('Successfully uploaded ' + req.files.length + ' files!');
});

app.get('/file/:filename', function (req, res) {
    var params = { Bucket: 'bucket_rebeil', Key: req.params.filename };

    s3.getObject(params, function(err, data) {
        if (err) {
            res.status(500).send(err);
        } else {
            res.send(data.Body);
        }
    });
});

app.listen(port, () => {
    console.log(`App listening at http://localhost:${port}`)
});
