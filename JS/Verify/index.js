const { verify } = require('../key-vault-client');
const multipart = require('parse-multipart');

module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');
    const bodyBuffer = Buffer.from(req.body);
    const boundary = multipart.getBoundary(req.headers['content-type']);
    const parts = multipart.Parse(bodyBuffer, boundary);

    context.res = {
        body: await verify(parts[0].data, req.query.signature),
    };
};
