import { Mail, Phone, MapPin } from 'lucide-react';

export default function ContactPage() {
  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <h1 className="text-4xl font-bold mb-8">Contact Us</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        <div>
          <h2 className="text-2xl font-bold mb-6">Get in Touch</h2>
          <div className="space-y-4">
            <div className="flex items-start">
              <Phone className="h-6 w-6 text-wekeza-blue mr-3 mt-1" />
              <div>
                <p className="font-semibold">Phone</p>
                <p className="text-gray-600">+254 700 000 000</p>
              </div>
            </div>
            <div className="flex items-start">
              <Mail className="h-6 w-6 text-wekeza-blue mr-3 mt-1" />
              <div>
                <p className="font-semibold">Email</p>
                <p className="text-gray-600">info@wekeza.com</p>
              </div>
            </div>
            <div className="flex items-start">
              <MapPin className="h-6 w-6 text-wekeza-blue mr-3 mt-1" />
              <div>
                <p className="font-semibold">Address</p>
                <p className="text-gray-600">Wekeza House, Nairobi, Kenya</p>
              </div>
            </div>
          </div>
        </div>
        <div className="card">
          <h3 className="text-xl font-bold mb-4">Send us a message</h3>
          <form className="space-y-4">
            <input type="text" placeholder="Your Name" className="input" />
            <input type="email" placeholder="Your Email" className="input" />
            <textarea placeholder="Your Message" rows={4} className="input"></textarea>
            <button type="submit" className="btn btn-primary w-full">Send Message</button>
          </form>
        </div>
      </div>
    </div>
  );
}
